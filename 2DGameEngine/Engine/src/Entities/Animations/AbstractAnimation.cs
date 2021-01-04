using GameEngine2D.Entities;
using GameEngine2D.src.Entities.Animation.Interface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.src.Entities.Animation
{
    abstract class AbstractAnimation : IAnimation
    {

        protected int CurrentFrame;
        private int totalFrames;
        private double delay = 0;
        private double currentDelay = 0;
        protected SpriteBatch SpriteBatch { get; set; }
        protected Entity Parent { get; set; }
        public float Scale = 0f;
        public Vector2 Offset = Vector2.Zero;
        protected SpriteEffects SpriteEffect { get; set; }
        protected Rectangle SourceRectangle;

        public AbstractAnimation(SpriteBatch spriteBatch, Rectangle sourceRectangle, Entity parent, int totalFrames, int framerate = 0, SpriteEffects spriteEffect = SpriteEffects.None)
        {
            this.SpriteBatch = spriteBatch;
            this.Parent = parent;
            CurrentFrame = 0;
            this.totalFrames = totalFrames;
            this.SpriteEffect = spriteEffect;
            this.SourceRectangle = sourceRectangle;
            if (framerate != 0)
            {
                delay = TimeSpan.FromSeconds(1).TotalMilliseconds / framerate;
            }
        }

        public abstract void Play();
        public void Update(GameTime gameTime)
        {
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
                CurrentFrame = 0;
            }
        }

        public void Init()
        {
            CurrentFrame = 0;
        }

        public void Stop()
        {
            CurrentFrame = 0;
        }
    }
}
