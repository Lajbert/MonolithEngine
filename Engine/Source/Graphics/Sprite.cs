﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MonolithEngine
{

    /// <summary>
    /// Represents a sprite (drawable texture) that can be assigned to an entity.
    /// </summary>
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
        public float Scale = 1f;

        public Color Color = Color.White;

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
                SourceRectangle = AssetUtil.AutoBoundingBox(this);
            }

            if (drawOffset == default)
            {
                offset = new Vector2(SourceRectangle.Width * owner.Pivot.X, SourceRectangle.Height * owner.Pivot.Y);
            }
            else
            {
                offset = drawOffset;
            }

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Owner.DrawPosition - offset, SourceRectangle, Color, Rotation, Origin, Scale, SpriteEffect, Owner.Depth);
        }

        public Type GetComponentType()
        {
            return GetType();
        }

        public static Texture2D Rectangle(int width, int height, Color color)
        {
            return AssetUtil.CreateRectangle(width, height, color);
        }

        public static Texture2D Square(int size, Color color)
        {
            return AssetUtil.CreateRectangle(size, size, color);
        }

        public static Texture2D Rectangle(int width, int height, Color[] color)
        {
            return AssetUtil.TextureFromColor(color, width, height);
        }

        public static Texture2D Square(int size, Color[] color)
        {
            return AssetUtil.TextureFromColor(color, size, size);
        }
    }
}
