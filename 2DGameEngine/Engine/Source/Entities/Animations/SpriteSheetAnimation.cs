using GameEngine2D.Entities;
using GameEngine2D.Source.Entities.Animation;
using GameEngine2D.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.Source.Entities
{
    public class SpriteSheetAnimation : AbstractAnimation
    {
        private Texture2D texture;
        private int rows;
        private int columns;
        private int width;
        private int height;

        private int currentRow;
        private int currentColumn;

        public SpriteSheetAnimation(Entity parent, Texture2D texture, int rows, int columns, int totalFrames, int width = 0, int height = 0, int framerate = 0, SpriteEffects spriteEffect = SpriteEffects.None) : base(parent, totalFrames, framerate, spriteEffect)
        {
            this.texture = texture;
            this.rows = rows;
            this.columns = columns;
            if (width == 0 || height == 0)
            {
                this.width = texture.Width / columns;
                this.height = texture.Height / rows;
            } else
            {
                this.width = width;
                this.height = height;
            }
        }

        /*public void Update(GameTime gameTime)
        {
            if (delay == 0)
            {
                currentFrame++;
            }
            else
            {
                if (currentDelay >= delay)
                {
                    currentFrame++;
                    currentDelay = 0;
                }
                else
                {
                    currentDelay += gameTime.ElapsedGameTime.TotalMilliseconds;
                }
            }

            if (currentFrame == totalFrames)
                currentFrame = 0;
        }*/

        protected override Texture2D GetTexture()
        {
            currentRow = (int)((float)CurrentFrame / (float)columns);
            currentColumn = CurrentFrame % columns;
            SourceRectangle = new Rectangle(width * currentColumn, height * currentRow, width, height);
            Pivot = new Vector2(width / 2, height / 2);
            //SourceRectangle = new Rectangle(height * currentRow, width * currentColumn, width, height);
            return texture;
        }

        /*public void Draw(SpriteBatch spriteBatch, Vector2 location)
        {
            int width = Texture.Width / Columns;
            int height = Texture.Height / Rows;
            int row = (int)((float)currentFrame / (float)Columns);
            int column = currentFrame % Columns;

            Rectangle sourceRectangle = new Rectangle(width * column, height * row, width, height);
            Rectangle destinationRectangle = new Rectangle((int)location.X, (int)location.Y, width, height);

            spriteBatch.Begin();
            spriteBatch.Draw(Texture, destinationRectangle, sourceRectangle, Color.White);
            spriteBatch.End();
        }*/
    }
}
