using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;

namespace MonolithEngine
{
    internal class AssetUtil
    {

        public static GraphicsDeviceManager GraphicsDeviceManager;
        public static ContentManager Content;

        private static Dictionary<RectangleKey, Texture2D> rectangleCache = new Dictionary<RectangleKey, Texture2D>();

        private static Random random = new Random();

        public static Texture2D CreateCircle(int diameter, Color color, bool filled = false)
        {
            Texture2D texture = new Texture2D(GraphicsDeviceManager.GraphicsDevice, diameter, diameter);
            Color[] colorData = new Color[diameter * diameter];

            float radius = diameter / 2f;
            float radiusSquared = radius * radius;

            for (int x = 0; x < diameter; x++)
            {
                for (int y = 0; y < diameter; y++)
                {
                    int index = x * diameter + y;
                    Vector2 pos = new Vector2(x - radius, y - radius);
                    if (filled)
                    {
                        if (pos.LengthSquared() <= radiusSquared)
                        {
                            colorData[index] = color;
                        }
                        else
                        {
                            colorData[index] = Color.Transparent;
                        }
                    } else
                    {
                        if (pos.LengthSquared() <= radiusSquared && pos.LengthSquared() > radiusSquared - 50)
                        {
                            colorData[index] = color;
                        }
                        else
                        {
                            colorData[index] = Color.Transparent;
                        }
                    }
                }
            }

            texture.SetData(colorData);
            return texture;
        }
        public static Texture2D CreateRectangle(int size, Color color)
        {
            if (rectangleCache.ContainsKey(new RectangleKey(size, color))) {
                return rectangleCache[new RectangleKey(size, color)];
            }
            Texture2D rect = new Texture2D(GraphicsDeviceManager.GraphicsDevice, size, size);
            Color[] data = new Color[size * size];
            for (int i = 0; i < data.Length; ++i) data[i] = color;
            rect.SetData(data);

            rectangleCache.Add(new RectangleKey(size, color), rect);
            return rect;
        }

        public static Texture2D CreateRectangle(int width, int height, Color color)
        {
            int key = 2 * width + height;
            if (rectangleCache.ContainsKey(new RectangleKey(key, color)))
            {
                return rectangleCache[new RectangleKey(key, color)];
            }
            Texture2D rect = new Texture2D(GraphicsDeviceManager.GraphicsDevice, width, height);
            Color[] data = new Color[width * height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int i = x + width * y;
                    data[i] = color;
                }
            }
            rect.SetData(data);

            rectangleCache.Add(new RectangleKey(key, color), rect);
            return rect;
        }

        public static List<Texture2D> LoadTextures(string fullPath, int frameCount)
        {
            return LoadTextures(fullPath, 0, frameCount);
        }

        /*public static Texture2D LoadTexturesWithMerge(string fullPath, int frameCount)
        {
            return LoadTexturesWithMerge(fullPath, 0, frameCount);
        }

        public static Texture2D LoadTexturesWithMerge(string fullPath, int startFrame, int endFrame)
        {
            List<Texture2D> textures = LoadTextures(fullPath, startFrame, endFrame);
            int resWidth = 0;
            int resHeight = 0;
            foreach (Texture2D texture in textures)
            {
                resWidth += texture.Width;
                resHeight += texture.Height;
            }
            //Color[] resultColor = new Color[resWidth * resHeight];
            int i = 0;
            Color[] currentColor;
            Texture2D result = new Texture2D(GraphicsDeviceManager.GraphicsDevice, resWidth, resHeight);
            foreach (Texture2D texture in textures)
            {
                currentColor = new Color[texture.Width * texture.Height];
                texture.GetData(currentColor);
                result.SetData(currentColor, i, currentColor.Length);
                //currentColor.CopyTo(resultColor, i);
                i += currentColor.Length + 1;
            }
            
            //result.SetData(resultColor);
            return result;
        }*/

        public static Texture2D FlipTexture(Texture2D input, bool vertical, bool horizontal)
        {
            Texture2D flipped = new Texture2D(GraphicsDeviceManager.GraphicsDevice, input.Width, input.Height);
            Color[] data = new Color[input.Width * input.Height];
            Color[] flipped_data = new Color[data.Length];

            input.GetData(data);

            for (int x = 0; x < input.Width; x++)
            {
                for (int y = 0; y < input.Height; y++)
                {
                    int index = 0;
                    if (horizontal && vertical)
                        index = input.Width - 1 - x + (input.Height - 1 - y) * input.Width;
                    else if (horizontal && !vertical)
                        index = input.Width - 1 - x + y * input.Width;
                    else if (!horizontal && vertical)
                        index = x + (input.Height - 1 - y) * input.Width;
                    else if (!horizontal && !vertical)
                        index = x + y * input.Width;

                    flipped_data[x + y * input.Width] = data[index];
                }
            }

            flipped.SetData(flipped_data);

            return flipped;
        }

        public static List<Texture2D> LoadTextures(string fullPath, int startFrame, int endFrame)
        {
            List<Texture2D> result = new List<Texture2D>();
            for (int i = startFrame; i <= endFrame; i++)
            {
                result.Add(Content.Load<Texture2D>(fullPath + i));
            }

            return result;
        }

        public static Texture2D LoadTexture(string path)
        {
            return Content.Load<Texture2D>(path);
        }

        public static  Color GetRandomColor()
        {
            return Color.FromNonPremultiplied(random.Next(0, 256), random.Next(0, 256), random.Next(0, 256), 256);
        }

        private class RectangleKey
        {
            public int size;
            public Color color;

            public RectangleKey(int size, Color color)
            {
                this.size = size;
                this.color = color;
            }

            public override bool Equals(object obj)
            {
                return obj is RectangleKey key &&
                       size == key.size &&
                       color.Equals(key.color);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(size, color);
            }
        }

        public static SoundEffect LoadSoundEffect(string path)
        {
            return Content.Load<SoundEffect>(path);
        }

        public static Song LoadSong(string path)
        {
            return Content.Load<Song>(path);
        }

        public static Rectangle AutoBoundingBox(Color[] inputImage, int imageWidth)
        {
            int left = int.MaxValue;
            int top = int.MaxValue;
            int right = int.MinValue;
            int bottom = int.MinValue;

            for (int i = 0; i < inputImage.Length; i++)
            {
                if (inputImage[i].ToVector4() != Vector4.Zero)
                {
                    int x = i % imageWidth;
                    int y = i / imageWidth;

                    if (x < left)
                    {
                        left = x;
                    }
                    if (x > right)
                    {
                        right = x;
                    }
                    if (y < top)
                    {
                        top = y;
                    }
                    if (y > bottom)
                    {
                        bottom = y;
                    }
                }
            }
            return new Rectangle(left, top, right - left + 1, bottom - top + 1);
        }

        public static Rectangle AutoBoundingBox(Texture2D texture)
        {
            int width = texture.Width;
            int height = texture.Height;
            Color[] data = new Color[width * height];
            texture.GetData(0, new Rectangle(0, 0, width, height), data, 0, data.Length);
            return AutoBoundingBox(data, width);
        }

        public static Rectangle AutoBoundingBox(Sprite sprite)
        {
            return AutoBoundingBox(sprite.Texture);
        }

        public static float AutoCircumscribedCircle(AbstractAnimation anim, BoundCalculationMode calculationMode = BoundCalculationMode.MAX)
        {
            return CircleFromRectangle(AutoBoundingBox(anim, calculationMode), calculationMode);
        }

        public static float AutoCircumscribedCircle(Texture2D texture, BoundCalculationMode calculationMode = BoundCalculationMode.MAX)
        {
            return CircleFromRectangle(AutoBoundingBox(texture), calculationMode);
        }

        private static float CircleFromRectangle(Rectangle rect, BoundCalculationMode calculationMode)
        {
            if (calculationMode == BoundCalculationMode.MAX)
            {
                return Math.Max(rect.Width, rect.Height);
            }
            else if (calculationMode == BoundCalculationMode.MIN)
            {
                return Math.Min(rect.Width, rect.Height);
            }
            else
            {
                return (rect.Width + rect.Height) / 2;
            }
        }

        public static float AutoCircumscribedCircle(Sprite sprite, BoundCalculationMode calculationMode)
        {
            return AutoCircumscribedCircle(sprite.Texture, calculationMode);
        }

        public static Rectangle AutoBoundingBox(AbstractAnimation anim, BoundCalculationMode calculationMode = BoundCalculationMode.MAX)
        {
            List<Rectangle> boundingBoxes = new List<Rectangle>();

            anim.Init();
            while (anim.CurrentFrame < anim.EndFrame)
            {
                boundingBoxes.Add(AutoBoundingBox(GetTextureArea(anim.GetTexture(), anim.SourceRectangle), anim.SourceRectangle.Width));
                anim.CurrentFrame++;
            }
            anim.Init();

            Rectangle result;

            if (calculationMode == BoundCalculationMode.MAX)
            {
                result = new Rectangle(int.MaxValue, int.MaxValue, int.MinValue, int.MinValue);
                foreach (Rectangle r in boundingBoxes)
                {
                    if (r.X < result.X)
                    {
                        result.X = r.X;
                    }
                    if (r.Y < result.Y)
                    {
                        result.Y = r.Y;
                    }
                    if (r.Width > result.Width)
                    {
                        result.Width = r.Width;
                    }
                    if (r.Height > result.Height)
                    {
                        result.Height = r.Height;
                    }
                }
            }
            else if (calculationMode == BoundCalculationMode.MIN)
            {
                result = new Rectangle(int.MinValue, int.MinValue, int.MaxValue, int.MaxValue);
                foreach (Rectangle r in boundingBoxes)
                {
                    if (r.X > result.X)
                    {
                        result.X = r.X;
                    }
                    if (r.Y > result.Y)
                    {
                        result.Y = r.Y;
                    }
                    if (r.Width < result.Width)
                    {
                        result.Width = r.Width;
                    }
                    if (r.Height < result.Height)
                    {
                        result.Height = r.Height;
                    }
                }
            }
            else
            {
                result = new Rectangle(0, 0, 0, 0);
                foreach (Rectangle r in boundingBoxes)
                {
                    result.X += r.X;
                    result.Y += r.Y;
                    result.Width += r.Width;
                    result.Height += r.Height;
                }

                result.X = (int)Math.Round((decimal)result.X / boundingBoxes.Count);
                result.Y = (int)Math.Round((decimal)result.Y / boundingBoxes.Count);
                result.Width = (int)Math.Round((decimal)result.Width / boundingBoxes.Count);
                result.Height = (int)Math.Round((decimal)result.Height / boundingBoxes.Count);
            }
            return result;
        }

        public static Color[] GetTextureArea(Texture2D texture, Rectangle rectangle)
        {
            int width = texture.Width;
            int height = texture.Height;
            Color[] colorData = new Color[width * height];
            texture.GetData(0, new Rectangle(0, 0, width, height), colorData, 0, colorData.Length);
            Color[] color = new Color[rectangle.Width * rectangle.Height];
            for (int x = 0; x < rectangle.Width; x++)
            {
                for (int y = 0; y < rectangle.Height; y++)
                {
                    color[x + y * rectangle.Width] = colorData[x + rectangle.X + (y + rectangle.Y) * width];
                }
            }
            return color;
        }

        public static Texture2D TextureFromColor(Color[] input, int width, int height)
        {
            Texture2D result = new Texture2D(GraphicsDeviceManager.GraphicsDevice, width, height);
            result.SetData(input);
            return result;
        }

        public enum BoundCalculationMode
        {
            MAX,
            MIN,
            AVERAGE
        }

    }
}
