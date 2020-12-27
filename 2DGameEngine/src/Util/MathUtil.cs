using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace _2DGameEngine.src.Util
{
    class MathUtil
    {
        public static Vector2 Round(Vector2 v)
        {
            return new Vector2((float)Math.Round(v.X), (float)Math.Round(v.Y));
        }
    }
}
