using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
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
    }
}
