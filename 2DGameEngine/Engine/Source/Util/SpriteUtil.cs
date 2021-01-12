using GameEngine2D.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GameEngine2D.Engine.Source.Util
{
    public class SpriteUtil
    {

        public static GraphicsDeviceManager GraphicsDeviceManager;
        public static ContentManager Content;

        private static Dictionary<RectangleKey, Texture2D> rectangleCache = new Dictionary<RectangleKey, Texture2D>();

        public static Texture2D CreateCircle(int radius, Color color)
        {
            Texture2D texture = new Texture2D(GraphicsDeviceManager.GraphicsDevice, radius, radius);
            Color[] colorData = new Color[radius * radius];

            float diam = radius / 2f;
            float diamsq = diam * diam;

            for (int x = 0; x < radius; x++)
            {
                for (int y = 0; y < radius; y++)
                {
                    int index = x * radius + y;
                    Vector2 pos = new Vector2(x - diam, y - diam);
                    if (pos.LengthSquared() <= diamsq)
                    {
                        colorData[index] = color;
                    }
                    else
                    {
                        colorData[index] = Color.Transparent;
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
            Random random = new Random();
            return Color.FromNonPremultiplied(random.Next(256), random.Next(256), random.Next(256), 256);
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
    }
}
