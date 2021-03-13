using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.Engine.Source.UI
{
    public class TextField : IUIElement
    {
        public SpriteFont Font;
        public Func<string> DataSource;
        public Vector2 Position;
        public Color Color = Color.Black;
        public float Scale;
        public float Rotation;
        public float Depth;

        public TextField(SpriteFont font, Func<string> dataSource, Vector2 position, Color color = default, float scale = 1f, float rotation = 0f, float depth = 1)
        {
            Font = font;
            DataSource = dataSource;
            Position = position;
            if (color != default)
            {
                Color = color;
            }
            Rotation = rotation;
            Scale = scale;
            Depth = depth;
        }

        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            //public void DrawString(SpriteFont spriteFont, string text, Vector2 position, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth);
            spriteBatch.DrawString(Font, DataSource.Invoke(), Position, Color, Rotation, Vector2.Zero, Scale, SpriteEffects.None, Depth);
        }

        public virtual void Update(GameTime gameTime)
        {
            
        }
    }
}
