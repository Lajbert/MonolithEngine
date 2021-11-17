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

        public MonolithTexture(Texture2D texture, Rectangle sourceRectangle = default, Rectangle boundingBox = default, bool cachePixels = false)
        {
            this.texture = texture;
            this.sourceRectangle = sourceRectangle == default ? new Rectangle(0, 0, texture.Width, texture.Height) : sourceRectangle;
            this.boundingBox = boundingBox == default ? AssetUtil.AutoBoundingBox(texture) : boundingBox;
            if (cachePixels)
            {
                InitPixels();
            }
        }

        private void InitPixels()
        {
            pixels = new Color[texture.Width * texture.Height];
            texture.GetData(pixels);
        }

        public Texture2D GetTexture2D()
        {
            return texture;
        }

        public Color[] GetPixels(Rectangle sourceRect = default)
        {
            bool deletePixels = pixels == null;
            if (pixels == null)
            {
                InitPixels();
            }
            if (sourceRect == default)
            {
                Color[] res = new Color[pixels.Length];
                Array.Copy(pixels, res, pixels.Length);
                if (deletePixels)
                {
                    pixels = null;
                }
                return res;
            }
            Color[] partial = new Color[sourceRect.Size.X * sourceRect.Size.Y];
            int i = 0;
            if (sourceRect.Size.Y - sourceRect.Y > 0)
            {
                for (int y = sourceRect.Y; y < sourceRect.Size.Y; y++)
                {
                    if (sourceRect.Size.X - sourceRect.X > 0)
                    {
                        for (int x = sourceRect.X; x < sourceRect.Size.X; x++)
                        {
                            //int idx = (y * sourceRect.Size.X) + x;
                            //int oneDindex = (row * length_of_row) + column; // Indexes
                            int idx = (y * sourceRect.Size.X) + x;
                            partial[i++] = pixels[idx];
                        }
                    }
                    else
                    {
                        int idx = (y * sourceRect.Size.X);
                        partial[i++] = pixels[idx];
                    }
                }
            }
            else
            {
                for (int x = sourceRect.X; x < sourceRect.Size.X; x++)
                {
                    //int idx = (y * sourceRect.Size.X) + x;
                    //int oneDindex = (row * length_of_row) + column; // Indexes
                    int idx = (0 * sourceRect.Size.X) + x;
                    partial[i++] = pixels[idx];
                }
            }
            if (deletePixels)
            {
                pixels = null;
            }
            return partial;
        }
    }
}
