using ForestPlatformerExample;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MonolithEngine
{
    /// <summary>
    /// Displays text on the UI.
    /// </summary>
    public class TextField : AbstractUIElement
    {
        public SpriteFont Font;
        public Func<string> DataSource;
        public Color Color = Color.Black;
        public float Scale;
        public float Rotation;
        public float Depth;

        public TextField(SpriteFont font, Func<string> dataSource, Vector2 position, Color color = default, float scale = 1f, float rotation = 0f, float depth = 1) : base (position)
        {
            Font = font;
            DataSource = dataSource;
            if (color != default)
            {
                Color = color;
            }
            Rotation = rotation;
            Scale = scale;
            Depth = depth;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!PlatformerGame.ANDROID)
            {
                //public void DrawString(SpriteFont spriteFont, string text, Vector2 position, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth);
                if (Parent == null)
                {
                    spriteBatch.DrawString(Font, DataSource.Invoke(), GetPosition(), Color, Rotation, Vector2.Zero, Scale, SpriteEffects.None, Depth);
                }
                else
                {
                    spriteBatch.DrawString(Font, DataSource.Invoke(), GetPosition(), Color, Rotation, Vector2.Zero, Scale, SpriteEffects.None, Depth);
                }
            }
        }
    }
}
