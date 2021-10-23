using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace MonolithEngine
{

    /// <summary>
    /// Abstract base class of different animation types.
    /// The core logic of playing animations here, the child
    /// classes are only responsible to parse different animation
    /// inputs (spritesheet or sprite from separate image) and to
    /// provide the next frame and source rectangle.
    /// </summary>
    public abstract class AbstractAnimation : IAnimation
    {

        internal int CurrentFrame;
        internal int TotalFrames;
        private double delay = 0;
        private double currentDelay = 0;
        public Color Color = Color.White;

        protected Entity Parent { get; set; }
        public float Scale = 1f;
        public Vector2 Offset = Vector2.Zero;
        protected SpriteEffects SpriteEffect { get; set; }

        public bool Looping = true;
        public bool Running = false;

        // action to call when the animations ends
        public Action StoppedCallback;
        // action to call when the animation is starrted
        public Action StartedCallback;
        // action to call when we switch from one animation to another
        // (the animation might not be finished before switch to a new one)
        public Action AnimationSwitchCallback;

        private bool stopActionCalled = false;
        private bool startActionCalled = false;

        // we will pause the current animation in the current frame when this
        // condition is true
        public Func<bool> AnimationPauseCondition;

        // this action will be called at every frame of the animation
        public Action<int> EveryFrameAction;

        // actions defined to be called on specific frames
        private Dictionary<int, Action<int>> frameActions = new Dictionary<int, Action<int>>();

        public Vector2 Origin;

        public int StartFrame = 0;
        public int EndFrame
        {
            get => TotalFrames;

            set => TotalFrames = value;
        }
        public float DrawPriority { get; set; }

        internal Rectangle SourceRectangle;

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
            delay = Config.FIXED_UPDATE_FPS / (float)framerate;
        }

        public bool Finished()
        {
            return !Running;
        }

        /// <summary>
        /// Makes a copy of an animation.
        /// </summary>
        /// <param name="anim"></param>
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

        /// <summary>
        /// Drawing the current frame.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public virtual void Play(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(GetTexture(), (Parent.DrawPosition + Offset), SourceRectangle, Color, Parent.DrawRotation, Origin, Scale, SpriteEffect, 0f);
        }

        internal abstract Texture2D GetTexture();

        /// <summary>
        /// Determining the next frame, calling different callbacks when needed.
        /// </summary>
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
                currentDelay -= delay;
            }
            else
            {
                currentDelay += Globals.FixedUpdateMultiplier;
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
