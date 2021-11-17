using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonolithEngine
{
    public class MonolithTexture
    {
        protected Texture2D texture;
        private Rectangle sourceRectangle;
        private Rectangle boundingBox;
        internal static GraphicsDevice GraphicsDevice;

        public MonolithTexture(Texture2D texture, Rectangle sourceRectangle = default, Rectangle boundingBox = default)
        {
            this.texture = texture;
            this.sourceRectangle = sourceRectangle == default ? TextureBorders() : sourceRectangle;
            this.boundingBox = boundingBox == default ? AssetUtil.AutoBoundingBox(texture) : boundingBox;
        }



        public Texture2D GetTexture2D()
        {
            return texture;
        }

        private Rectangle TextureBorders()
        {
            return new Rectangle(0, 0, texture.Width, texture.Height);
        }

        public Rectangle GetSourceRectangle()
        {
            return sourceRectangle;
        }

        public Rectangle GetBoundingBox()
        {
            return boundingBox;
        }
    }
}
