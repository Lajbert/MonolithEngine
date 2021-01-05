using GameEngine2D.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GameEngine2D.Engine.src.Util
{
    public class SpriteUtil
    {
        public static Texture2D CreateCircle(GraphicsDeviceManager graphics, int radius, Color color)
        {
            Texture2D texture = new Texture2D(graphics.GraphicsDevice, radius, radius);
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
        public static Texture2D CreateRectangle(GraphicsDeviceManager graphics, int size, Color color)
        {
            Texture2D rect = new Texture2D(graphics.GraphicsDevice, size, size);
            Color[] data = new Color[size * size];
            for (int i = 0; i < data.Length; ++i) data[i] = color;
            rect.SetData(data);
            return rect;
        }

        public static List<Texture2D> LoadTextures(string fullPath, int frames, ContentManager content)
        {
            return LoadTextures(fullPath, 0, frames, content);
        }

        public static List<Texture2D> LoadTextures(string fullPath, int startFrame, int endFrame, ContentManager content)
        {
            List<Texture2D> result = new List<Texture2D>();
            for (int i = startFrame; i <= endFrame; i++)
            {
                result.Add(content.Load<Texture2D>(fullPath + i));
            }

            return result;
        }
    }
}
