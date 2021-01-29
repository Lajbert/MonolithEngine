using GameEngine2D.Entities;
using GameEngine2D.Global;
using GameEngine2D.Source.Entities.Animation.Interface;
using GameEngine2D.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.Source.Entities.Animation
{
    public abstract class AbstractAnimation : IAnimation
    {

        protected int CurrentFrame;
        private int totalFrames;
        private double delay = 0;
        private double currentDelay = 0;
        protected Entity Parent { get; set; }
        public float Scale = 1f;
        public Vector2 Offset = Vector2.Zero;
        protected SpriteEffects SpriteEffect { get; set; }
        public bool Looping = true;
        public bool Running = false;
        public Action StoppedCallback;
        public Action StartedCallback;

        public Func<bool> AnimationPauseCondition;
        public Action<int> EveryFrameAction;
        private Dictionary<int, Action<int>> frameActions = new Dictionary<int, Action<int>>();

        public Vector2 Pivot;

        public int StartFrame = 0;
        public int EndFrame
        {
            get => totalFrames;

            set => totalFrames = value;
        }

        protected Rectangle SourceRectangle;

        public AbstractAnimation(Entity parent, int totalFrames, int framerate = 0, SpriteEffects spriteEffect = SpriteEffects.None, Action startCallback = null, Action stopCallback = null)
        {
            this.Parent = parent;
            CurrentFrame = StartFrame;
            this.totalFrames = totalFrames;
            this.SpriteEffect = spriteEffect;
            this.StartedCallback = startCallback;
            this.StoppedCallback = stopCallback;
            if (framerate != 0)
            {
                delay = TimeSpan.FromSeconds(1).TotalMilliseconds / framerate;
            }
        }

        public bool Finished()
        {
            return !Running;
        }

        protected void Copy(AbstractAnimation anim)
        {
            anim.Looping = Looping;
            anim.Scale = Scale;
            anim.Offset = Offset;
            anim.delay = delay;
            anim.SpriteEffect = SpriteEffect;
            anim.totalFrames = totalFrames;
            anim.StartFrame = StartFrame;
            anim.EndFrame = EndFrame;
            anim.EveryFrameAction = EveryFrameAction;
            anim.frameActions = frameActions;
            anim.StoppedCallback = StoppedCallback;
            anim.StartedCallback = StartedCallback;
        }

        public virtual void Play(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(GetTexture(), (Parent.GetPositionWithParent() + Offset), SourceRectangle, Color.White, 0f, Pivot, Scale, SpriteEffect, 0f);
        }

        protected abstract Texture2D GetTexture();

        public void Update(GameTime gameTime)
        {
            if (!Running)
            {
                return;
            }

            //pausing the current animation by not incrementing the CurrentFrame
            if (AnimationPauseCondition != null && AnimationPauseCondition.Invoke())
            {
                return;
            }

            if (CurrentFrame == StartFrame)
            {
                if (!Looping)
                {
                    StartedCallback?.Invoke();
                }
            }

            if (delay == 0)
            {
                CurrentFrame++;
            }
            else
            {
                if (currentDelay >= delay)
                {
                    CurrentFrame++;
                    EveryFrameAction?.Invoke(CurrentFrame);
                    if (frameActions.ContainsKey(CurrentFrame))
                    {
                        frameActions[CurrentFrame].Invoke(CurrentFrame);
                    }
                    currentDelay = 0;
                }
                else
                {
                    currentDelay += gameTime.ElapsedGameTime.TotalMilliseconds;
                }
            }

            if (CurrentFrame == totalFrames) {
                if (!Looping)
                {
                    Stop();
                    StoppedCallback?.Invoke();
                }
                else
                {
                    Init();
                }
            }
        }

        public int GetCurrentFrame()
        {
            return CurrentFrame;
        }

        public void Init(int? startFrame = null)
        {
            if (startFrame == null)
            {
                CurrentFrame = StartFrame;
            } else
            {
                CurrentFrame = (int)startFrame;
            }
            Running = true;
        }

        public void Stop()
        {
            CurrentFrame = totalFrames - 1;
            Running = false;
        }

        public void Flip()
        {
            if (this.SpriteEffect == SpriteEffects.None)
            {
                SpriteEffect = SpriteEffects.FlipHorizontally;
            } else
            {
                SpriteEffect = SpriteEffects.None;
            }
        }

        public void AddFrameAction(int frame, Action<int> action)
        {
            if (frameActions.ContainsKey(frame))
            {
                frameActions[frame] += action;
            } else
            {
                frameActions.Add(frame, action);
            }
        }
    }
}
