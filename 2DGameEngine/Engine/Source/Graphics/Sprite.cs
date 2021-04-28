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
        public float Rotation = 0f;
        public Vector2 Origin;
        private Vector2 offset = Vector2.Zero;

        public Sprite(Entity owner, Texture2D texture, Rectangle sourceRectangle = default, Vector2 drawOffset = default, float rotation = 0f, Vector2 origin = default, bool flipHorizontal = false, bool flipVertical = false)
        {
            if (flipHorizontal || flipVertical)
            {
                Texture = AssetUtil.FlipTexture(texture, flipVertical, flipHorizontal);
            } 
            else
            {
                Texture = texture;
            }

            UniquePerEntity = true;
            DrawOffset = drawOffset;
            Owner = owner;
            Rotation = rotation;
            Origin = origin;

            if (sourceRectangle != default)
            {
                SourceRectangle = sourceRectangle;
            }
            else
            {
                //SourceRectangle = new Rectangle(0, 0, Config.GRID, Config.GRID);
                SourceRectangle = AssetUtil.AutoBoundingBox(this);
            }

            offset = new Vector2(SourceRectangle.Width * owner.Pivot.X, SourceRectangle.Height * owner.Pivot.Y);

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Owner.DrawPosition - offset, SourceRectangle, Color.White, Rotation, Origin, 1f, SpriteEffect, Owner.Depth);
        }
    }
}
