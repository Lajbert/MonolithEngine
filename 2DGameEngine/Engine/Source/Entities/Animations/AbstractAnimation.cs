using MonolithEngine.Engine.Source.Global;
using MonolithEngine.Entities;
using MonolithEngine.Global;
using MonolithEngine.Source.Entities.Animation.Interface;
using MonolithEngine.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonolithEngine.Source.Entities.Animation
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

        private float fixedUpdateMillisec;

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
            delay = (float)TimeSpan.FromTicks((long)(TimeSpan.TicksPerSecond / framerate)).TotalMilliseconds;
            fixedUpdateMillisec = (float)TimeSpan.FromTicks((long)(TimeSpan.TicksPerSecond / Config.FIXED_UPDATE_FPS)).TotalMilliseconds;
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
            spriteBatch.Draw(GetTexture(), (Parent.DrawPosition + Offset), SourceRectangle, Color.White, Parent.DrawRotation, Pivot, Scale, SpriteEffect, 0f);
        }

        protected abstract Texture2D GetTexture();

        public void Update()
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
                    InvokeStartedCallback();
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
                currentDelay += Globals.FixedUpdateMultiplier * fixedUpdateMillisec;
            }

            if (CurrentFrame == TotalFrames) {
                if (!Looping)
                {
                    Stop();
                    InvokeStoppedCallback();
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

        public void InvokeStoppedCallback()
        {
            if (!stopActionCalled)
            {
                StoppedCallback?.Invoke();
                stopActionCalled = true;
            }
        }

        public void InvokeStartedCallback()
        {
            if (!startActionCalled)
            {
                StartedCallback?.Invoke();
                startActionCalled = true;
            }
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
