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
    class SpriteUtil
    {
        public static Texture2D CreateRectangle(GraphicsDeviceManager graphics, int size, Color color)
        {
            Texture2D rect = new Texture2D(graphics.GraphicsDevice, size, size);
            Color[] data = new Color[size * size];
            for (int i = 0; i < data.Length; ++i) data[i] = color;
            rect.SetData(data);
            return rect;
        }

        public static List<Texture2D> LoadTextures(string path, string filePrefix, int frames, ContentManager content)
        {
            List<Texture2D> result = new List<Texture2D>();
            for (int i = 0; i <= frames; i++)
            {
                result.Add(content.Load<Texture2D>(path + filePrefix + i));
            }

            return result;
        }
    }
}
