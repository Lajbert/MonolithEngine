using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonolithEngine
{
    /// <summary>
    /// An image to disply on the UI.
    /// </summary>
    public class Image : AbstractUIElement
    {
        public Rectangle SourceRectangle;
        public float Scale = 1f;
        public float Rotation = 0f;
        public Texture2D ImageTexture;
        public SpriteEffects SpriteEffect = SpriteEffects.None;
        public int Depth = 1;
        public Color Color = Color.White;

        public Image(Texture2D texture, Vector2 position = default, Rectangle sourceRectangle = default, float scale = 1f, float rotation = 0f, int depth = 1, Color color = default) : base (position)
        {
            ImageTexture = texture;
            if (sourceRectangle == default && ImageTexture != null)
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

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(ImageTexture, GetPosition(), SourceRectangle, Color, Rotation, Vector2.Zero, Scale, SpriteEffect, Depth);
        }
    }
}
