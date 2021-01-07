using GameEngine2D.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GameEngine2D.Engine.Source.Util
{
    public class SpriteUtil
    {

        public static GraphicsDeviceManager GraphicsDeviceManager;
        public static ContentManager Content;
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
            Texture2D rect = new Texture2D(GraphicsDeviceManager.GraphicsDevice, size, size);
            Color[] data = new Color[size * size];
            for (int i = 0; i < data.Length; ++i) data[i] = color;
            rect.SetData(data);
            return rect;
        }

        public static List<Texture2D> LoadTextures(string fullPath, int frames)
        {
            return LoadTextures(fullPath, 0, frames);
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
            Random random = new Random();
            return Color.FromNonPremultiplied(random.Next(256), random.Next(256), random.Next(256), 256);
        }
    }
}
