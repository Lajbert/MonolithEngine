using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.src.Util
{
    class MathUtil
    {
        public static Vector2 Round(Vector2 v)
        {
            return new Vector2((float)Math.Round(v.X), (float)Math.Round(v.Y));
        }

		public static float Distance(float ax, float ay, float bx, float by)
		{
			return (float)Math.Sqrt(SquaredDistance(ax, ay, bx, by));
		}

		public static float SquaredDistance(float ax, float ay, float bx, float by)
		{
			return (ax - bx) * (ax - bx) + (ay - by) * (ay - by);
		}
	}
}
