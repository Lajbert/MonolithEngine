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
        protected int TotalFrames;
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
        public Action AnimationSwitchCallback;

        private bool stopActionCalled = false;
        private bool startActionCalled = false;

        public Func<bool> AnimationPauseCondition;
        public Action<int> EveryFrameAction;
        private Dictionary<int, Action<int>> frameActions = new Dictionary<int, Action<int>>();

        public Vector2 Pivot;

        public int StartFrame = 0;
        public int EndFrame
        {
            get => TotalFrames;

            set => TotalFrames = value;
        }
        public float DrawPriority { get; set; }

        protected Rectangle SourceRectangle;

        public AbstractAnimation(Entity parent, int totalFrames, int framerate, SpriteEffects spriteEffect = SpriteEffects.None, Action startCallback = null, Action stopCallback = null)
        {
            if (framerate < 1)
            {
                throw new Exception("Invalid framerate!");
            }
            this.Parent = parent;
            CurrentFrame = StartFrame;
            this.TotalFrames = totalFrames;
            this.SpriteEffect = spriteEffect;
            this.StartedCallback = startCallback;
            this.StoppedCallback = stopCallback;
            delay = TimeSpan.FromSeconds(1).TotalMilliseconds / framerate;
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
            anim.TotalFrames = TotalFrames;
            anim.StartFrame = StartFrame;
            anim.EndFrame = EndFrame;
            anim.EveryFrameAction = EveryFrameAction;
            anim.frameActions = frameActions;
            anim.StoppedCallback = StoppedCallback;
            anim.StartedCallback = StartedCallback;
            anim.AnimationSwitchCallback = AnimationSwitchCallback;
        }

        public virtual void Play(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(GetTexture(), (Parent.Transform.Position + Offset), SourceRectangle, Color.White, 0f, Pivot, Scale, SpriteEffect, 0f);
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
                if (!Looping && !startActionCalled)
                {
                    StartedCallback?.Invoke();
                    startActionCalled = true;
                }
            }

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

            if (CurrentFrame == TotalFrames) {
                if (!Looping)
                {
                    Stop();
                    if (!stopActionCalled)
                    {
                        StoppedCallback?.Invoke();
                        stopActionCalled = true;
                    }
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
            startActionCalled = false;
            stopActionCalled = false;
        }

        public void Stop()
        {
            CurrentFrame = TotalFrames - 1;
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

        public abstract void Destroy();
    }
}
