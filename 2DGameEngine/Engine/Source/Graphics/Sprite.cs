using MonolithEngine.Engine.Source.Components;
using MonolithEngine.Engine.Source.Interfaces;
using MonolithEngine.Entities;
using MonolithEngine.Global;
using MonolithEngine.Source.GridCollision;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using MonolithEngine.Engine.Source.Util;

namespace MonolithEngine.Engine.Source.Graphics
{
    public class Sprite : IComponent, IDrawableComponent
    {
        public bool UniquePerEntity { get; set; }
        public Texture2D Texture;
        public Rectangle SourceRectangle;
        public Vector2 DrawOffset;
        public Entity Owner;
        public SpriteEffects SpriteEffect = SpriteEffects.None;

        public Sprite(Entity owner, Texture2D texture, Rectangle? sourceRectangle = null, Vector2 drawOffset = default, bool flipHorizontal = false, bool flipVertical = false)
        {
            if (flipHorizontal || flipVertical)
            {
                Texture = AssetUtil.FlipTexture(texture, flipVertical, flipHorizontal);
            } 
            else
            {
                Texture = texture;
            }

            if (sourceRectangle.HasValue)
            {
                SourceRectangle = sourceRectangle.Value;
            } 
            else
            {
                SourceRectangle = new Rectangle(0, 0, Config.GRID, Config.GRID);
            }
            
            UniquePerEntity = true;
            DrawOffset = drawOffset;
            Owner = owner;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Owner.DrawPosition + DrawOffset, SourceRectangle, Color.White, 0f, Owner.Pivot, 1f, SpriteEffect, Owner.Depth);
        }
    }
}
