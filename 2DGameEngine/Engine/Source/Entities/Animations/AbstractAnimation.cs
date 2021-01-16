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
        protected bool Started = false;
        public Action StoppedAction;
        public Action StartedAction;
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
            this.StartedAction = startCallback;
            this.StoppedAction = stopCallback;
            if (framerate != 0)
            {
                delay = TimeSpan.FromSeconds(1).TotalMilliseconds / framerate;
            }
        }

        public bool Finished()
        {
            return CurrentFrame == StartFrame && !Started;
        }

        public virtual void Play(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(GetTexture(), (Parent.GetPositionWithParent() + Offset), SourceRectangle, Color.White, 0f, Pivot, Scale, SpriteEffect, 0f);
        }

        protected abstract Texture2D GetTexture();

        public void Update(GameTime gameTime)
        {
            if (!Started)
            {
                return;
            }

            if (CurrentFrame == StartFrame)
            {
                if (StartedAction != null && !Looping)
                {
                    StartedAction.Invoke();
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
                    if (StoppedAction != null)
                    {
                        StoppedAction.Invoke();
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
            Started = true;
        }

        public void Stop()
        {
            CurrentFrame = StartFrame;
            Started = false;
        }

        public void StartPlayingAt(int frame)
        {

        }
    }
}
