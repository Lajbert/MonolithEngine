using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonolithEngine
{
    public class MonolithTexture
    {
        private Texture2D texture;
        private Rectangle sourceRectangle;
        private Rectangle boundingBox;
        private Color[] pixels;
        public static GraphicsDevice GraphicsDevice;

        public MonolithTexture(Texture2D texture, Rectangle sourceRectangle = default, Rectangle boundingBox = default, bool cachePixels = false)
        {
            this.texture = texture;
            this.sourceRectangle = sourceRectangle == default ? TextureBorders() : sourceRectangle;
            this.boundingBox = boundingBox == default ? AssetUtil.AutoBoundingBox(texture) : boundingBox;
            if (cachePixels)
            {
                InitPixels();
            }
        }

        private void InitPixels()
        {
            RenderTarget2D renderTarget = new RenderTarget2D(
                           GraphicsDevice,
                           sourceRectangle.Width,
                           sourceRectangle.Height,
                           false,
                           GraphicsDevice.PresentationParameters.BackBufferFormat,
                           DepthFormat.Depth24);
            GraphicsDevice.SetRenderTarget(renderTarget);

            GraphicsDevice.Clear(Color.Transparent);

            SpriteBatch spriteBatch = new SpriteBatch(GraphicsDevice);
            spriteBatch.Begin();
            spriteBatch.Draw(texture, sourceRectangle, Color.White);
            spriteBatch.End();
            GraphicsDevice.SetRenderTarget(null);
            pixels = new Color[renderTarget.Width * renderTarget.Height];
            renderTarget.GetData(pixels);
        }

        public Texture2D GetTexture2D()
        {
            return texture;
        }

        private Rectangle TextureBorders()
        {
            return new Rectangle(0, 0, texture.Width, texture.Height);
        }

        public Color[] GetPixels(Rectangle sourceRect = default)
        {
            bool deletePixels = pixels == null;
            if (pixels == null)
            {
                InitPixels();
            }
            if (sourceRect == TextureBorders() || sourceRect == default)
            {
                Color[] res = new Color[pixels.Length];
                Array.Copy(pixels, res, pixels.Length);
                if (deletePixels)
                {
                    pixels = null;
                }
                return res;
            }

            Color[] partial = AssetUtil.GetPixels(pixels, sourceRect, texture.Width);

            if (deletePixels)
            {
                pixels = null;
            }
            return partial;
        }

        public Rectangle GetSourceRectangle()
        {
            return sourceRectangle;
        }
    }
}
