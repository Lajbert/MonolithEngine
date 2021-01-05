using GameEngine2D.Entities;
using GameEngine2D.Global;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.src.Entities.Animation
{
    public class AnimatedSpriteGroup : AbstractAnimation
    {
        public List<Texture2D> Textures { get; set; }

        public AnimatedSpriteGroup(List<Texture2D> textures, Entity parent, SpriteBatch spriteBatch, Rectangle sourceRectangle, int framerate = 0, SpriteEffects spriteEffect = SpriteEffects.None) : base(spriteBatch, sourceRectangle, parent, textures.Count, framerate, spriteEffect)
        {
            this.Textures = textures;
        }

        protected override Texture2D GetTexture()
        {
            return Textures[CurrentFrame];
        }

       /* public override void Play()
        {
            Texture2D texture = Textures[CurrentFrame];
            int width = Config.GRID;
            int height = Config.GRID;
            //Rectangle destinationRectangle = new Rectangle((int)position.X, (int)position.Y, width, height);

            //spriteBatch.Begin();
            //SpriteBatch.Begin(SpriteSortMode, BlendState, SamplerState.PointClamp, DepthStencilState, RasterizerState)
            SpriteBatch.Begin(SpriteSortMode.Texture, null, SamplerState.PointClamp, null, null);
            //public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth);
            SpriteBatch.Draw(texture, Parent.GetPositionWithParent() + Offset, SourceRectangle, Color.White, 0f, Vector2.Zero, Scale, SpriteEffect, 0f);
            //spriteBatch.Draw(texture, destinationRectangle, sourceRectangle, Color.White);
            SpriteBatch.End();
        }*/
    }
}
