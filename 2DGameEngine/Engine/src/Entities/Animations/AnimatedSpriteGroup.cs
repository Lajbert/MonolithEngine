using GameEngine2D.Entities;
using GameEngine2D.Global;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.src.Entities.Animation
{
    class AnimatedSpriteGroup : AbstractAnimation
    {
        public List<Texture2D> textures { get; set; }

        public AnimatedSpriteGroup(List<Texture2D> textures, Entity parent, SpriteBatch spriteBatch, int framerate = 0, SpriteEffects spriteEffect = SpriteEffects.None) : base(spriteBatch, parent, textures.Count, framerate, spriteEffect)
        {
            this.textures = textures;
        }

        public override void Draw(Vector2 position)
        {
            Texture2D texture = textures[currentFrame];
            int width = Constants.GRID;
            int height = Constants.GRID;

            Rectangle sourceRectangle = new Rectangle(0, 0, 100, 55);
            //Rectangle destinationRectangle = new Rectangle((int)position.X, (int)position.Y, width, height);

            spriteBatch.Begin();
            //public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth);
            spriteBatch.Draw(texture, position + offset, sourceRectangle, Color.White, 0f, Vector2.Zero, scale, spriteEffect, 0f);
            //spriteBatch.Draw(texture, destinationRectangle, sourceRectangle, Color.White);
            spriteBatch.End();
        }
    }
}
