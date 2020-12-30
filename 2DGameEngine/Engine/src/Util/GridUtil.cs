using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.Util
{
    interface GridUtil
    {
        public static Vector2 GetRightGrid(Vector2 coord)
        {
            return new Vector2(coord.X + 1, coord.Y);
        }

        public static Vector2 GetLeftGrid(Vector2 coord)
        {
            return new Vector2(coord.X - 1, coord.Y);
        }

        public static Vector2 GetUpperGrid(Vector2 coord)
        {
            return new Vector2(coord.X, coord.Y - 1);
        }

        public static Vector2 GetBelowGrid(Vector2 coord)
        {
            return new Vector2(coord.X, coord.Y + 1);
        }
    }
}
