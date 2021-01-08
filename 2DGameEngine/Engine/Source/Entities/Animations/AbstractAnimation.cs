using GameEngine2D.Entities;
using GameEngine2D.Global;
using GameEngine2D.Source.Entities.Animation.Interface;
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
        public float Scale = 0f;
        public Vector2 Offset = Vector2.Zero;
        protected SpriteEffects SpriteEffect { get; set; }
        public bool Looping = true;
        protected bool Started = false;
        public Action StoppedAction;
        public Action StartedAction;
        public Vector2 Pivot;

        public AbstractAnimation(Entity parent, int totalFrames, int framerate = 0, SpriteEffects spriteEffect = SpriteEffects.None, Action startCallback = null, Action stopCallback = null)
        {
            this.Parent = parent;
            CurrentFrame = 0;
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
            return CurrentFrame == 0 && !Started;
        }

        public virtual void Play(SpriteBatch spriteBatch)
        {
            Pivot = new Vector2((float)Math.Floor((decimal)GetTexture().Width / 2), (float)Math.Floor((decimal)GetTexture().Height / 2));
            //Rectangle destinationRectangle = new Rectangle((int)position.X, (int)position.Y, width, height);

            //spriteBatch.Begin();
            //SpriteBatch.Begin(SpriteSortMode, BlendState, SamplerState.PointClamp, DepthStencilState, RasterizerState)
            //SpriteBatch.Begin(SpriteSortMode.Texture, null, SamplerState.PointClamp, null, null);
            //public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth);
            //SpriteBatch.Draw(GetTexture(), Parent.GetPositionWithParent() + Offset, SourceRectangle, Color.White, 0f, Vector2.Zero, Scale, SpriteEffect, 0f);
            spriteBatch.Draw(GetTexture(), (Parent.GetPositionWithParent() + Offset) * new Vector2(Config.SCALE, Config.SCALE), new Rectangle(0, 0, GetTexture().Width, GetTexture().Height), Color.White, 0f, Pivot, Scale * Config.SCALE, SpriteEffect, 0f);
            //spriteBatch.Draw(texture, destinationRectangle, sourceRectangle, Color.White);
            //SpriteBatch.End();
        }

        protected abstract Texture2D GetTexture();

        public void Update(GameTime gameTime)
        {
            if (!Started)
            {
                return;
            }

            if (CurrentFrame == 0)
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

        public void Init()
        {
            CurrentFrame = 0;
            Started = true;
        }

        public void Stop()
        {
            CurrentFrame = 0;
            Started = false;
        }
    }
}
