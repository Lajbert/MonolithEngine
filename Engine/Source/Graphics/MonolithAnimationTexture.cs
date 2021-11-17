using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonolithEngine
{
    public class AnimationTexture : MonolithTexture
    {
        internal int Rows;
        internal int Columns;
        internal int Width;
        internal int Height;
        private int frameCount;

        public int FrameCount
        {
            get => frameCount;
        }

        private int frameSize;


        public AnimationTexture(Texture2D texture, int frameSizeOverride = 0, int rows = 0, int columns = 0, int totalFrames = 0, int width = 0, int height = 0) : base(texture)
        {

            if (frameSizeOverride == 0)
            {
                frameSize = GetFrameSize();
            }
            else
            {
                frameSize = frameSizeOverride;
            }
            this.Rows = rows == 0 ? texture.Height / frameSize : rows;
            this.Columns = columns == 0 ? texture.Width / frameSize : columns;
            this.Width = width == 0 ? frameSize : width;
            this.Height = height == 0 ? frameSize : height;

            if (totalFrames == 0)
            {
                RenderTarget2D renderTarget = new RenderTarget2D(
                   GraphicsDevice,
                   texture.Width,
                   texture.Height,
                   false,
                   GraphicsDevice.PresentationParameters.BackBufferFormat,
                   DepthFormat.Depth24);

                GraphicsDevice.SetRenderTarget(renderTarget);

                GraphicsDevice.Clear(Color.Transparent);

                SpriteBatch spriteBatch = new SpriteBatch(GraphicsDevice);
                spriteBatch.Begin();
                spriteBatch.Draw(texture, new Rectangle(0, 0, texture.Width, texture.Height), Color.White);
                spriteBatch.End();
                GraphicsDevice.SetRenderTarget(null);
                Color[] pixels = new Color[renderTarget.Width * renderTarget.Height];
                renderTarget.GetData(pixels);

                frameCount = GetFrameCount(pixels, rows, columns);
            }
            else
            {
                frameCount = totalFrames;
            }
        }

        private int GetFrameCount(Color[] allPixels, int rows, int columns)
        {
            int frameCount = 0;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    Color[] data = AssetUtil.GetPixels(allPixels, new Rectangle(j * Width, i * Height, Width, Height), texture.Width);
                    bool emptyFrameFound = true;
                    for (int c = 0; c < Width * Height; c++)
                    {
                        if (data[c].ToVector4() != Vector4.Zero)
                        {
                            emptyFrameFound = false;
                            break;
                        }
                    }
                    if (emptyFrameFound)
                    {
                        return frameCount;
                    }
                    frameCount++;
                }
            }
            return frameCount;
        }

        private int GetFrameSize()
        {
            int longerSide = Math.Max(texture.Width, texture.Height);

            int biggestFrame = 0;

            for (int i = 1; i <= Math.Log(longerSide); i++)
            {
                int pow = (int)Math.Pow(2, i);
                if (texture.Width % pow == 0 && texture.Height % pow == 0)
                {
                    biggestFrame = pow;
                }
            }
            if (biggestFrame == 0)
            {
                throw new Exception("Can't determine frame size, the image dimensions are not the multiples of power of 2");
            }
            return biggestFrame;
        }
    }
}
