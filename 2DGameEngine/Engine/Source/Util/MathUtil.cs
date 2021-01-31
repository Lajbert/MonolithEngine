using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.Source.Util
{
    public class MathUtil
    {

		public static Vector2 RadToVector(float angleRad)
        {
			return new Vector2((float)Math.Cos(angleRad), (float)Math.Sin(angleRad));
        }

		public static Vector2 EndPointOfLine(Vector2 start, float length, float angleRad)
        {
			return new Vector2(start.X + length * (float)Math.Cos(angleRad), start.Y + length * (float)Math.Sin(angleRad));
		}

		public static float DegreesToRad(float angle)
        {
			return (float)(Math.PI / 180) * angle;
		}

		public static float RadFromVectors(Vector2 v1, Vector2 v2)
        {
			return (float)Math.Atan2(v2.Y - v1.Y, v2.X - v1.X);
		}

		public static float DegreeFromVectors(Vector2 v1, Vector2 v2)
        {
			return (float)(RadFromVectors(v1, v2) * 180 / Math.PI); 
		}

		public static Vector2 Abs(Vector2 v)
        {
			return new Vector2(Math.Abs(v.X), Math.Abs(v.Y));
        }

		public static bool SmallerEqualAbs(Vector2 a, Vector2 b)
        {
			Vector2 v1 = Abs(a);
			Vector2 v2 = Abs(b);
			return v1.X <= v2.X && v1.Y < v2.Y;
        }

        public static Vector2 Round(Vector2 v)
        {
            return new Vector2((float)Math.Round(v.X), (float)Math.Round(v.Y));
        }

		public static float Clamp(float x, float min, float max)
		{
			return (x < min) ? min : (x > max) ? max : x;
		}
	}
}
