using MonolithEngine.Engine.Source.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using MonolithEngine.Global;

namespace MonolithEngine.Engine.Source.UI
{
    public class Image : IUIElement
    {
        public Vector2 Position;
        public Rectangle SourceRectangle;
        public float Scale = 1f;
        public float Rotation = 0f;
        public Texture2D ImageTexture;
        public SpriteEffects SpriteEffect = SpriteEffects.None;
        public int Depth = 1;
        public Color Color = Color.White;

        public Image(Texture2D texture, Vector2 position = default, Rectangle sourceRectangle = default, float scale = 1f, float rotation = 0f, int depth = 1, Color color = default)
        {
            ImageTexture = texture;
            Position = position;
            if (sourceRectangle == default)
            {
                SourceRectangle = new Rectangle(0, 0, ImageTexture.Width, ImageTexture.Height);
            }
            Scale = scale;
            Rotation = rotation;
            Depth = depth;
            if (color != default)
            {
                Color = color;
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(ImageTexture, Position, SourceRectangle, Color, Rotation, Vector2.Zero, Scale * Config.ZOOM, SpriteEffect, Depth);
        }

        public virtual void Update(Point mousePosition = default)
        {

        }
    }
}
