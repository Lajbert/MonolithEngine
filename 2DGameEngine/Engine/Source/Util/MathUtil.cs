using MonolithEngine.Global;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonolithEngine.Source.Util
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

		public static Vector2 CalculateGridCoordintes(Vector2 position)
        {
			return new Vector2((int)Math.Floor(position.X / Config.GRID), (int)Math.Floor(position.Y / Config.GRID));
		}

        public static Vector2 CalculateInCellLocation(Vector2 position)
        {
            Vector2 pos = position / Config.GRID;
            return new Vector2(pos.X - (float)Math.Truncate(pos.X), pos.Y - (float)Math.Truncate(pos.Y));
        }

        public static float LerpRotationDegrees(float from, float to, float alpha)
        {
            if (alpha == 0) return from;
            if (from == to || alpha == 1) return to;

            Vector2 fromV = new Vector2((float)Math.Cos(from), (float)Math.Sin(from));
            Vector2 toV = new Vector2((float)Math.Cos(to), (float)Math.Sin(to));

            Vector2 interpolated = LerpRorationVectors(fromV, toV, alpha);

            return (float)Math.Atan2(interpolated.Y, interpolated.X);
        }

        public static Vector2 LerpRorationVectors(Vector2 from, Vector2 to, float alpha)
        {
            if (alpha == 0) return from;
            if (from == to || alpha == 1) return to;

            double theta = Math.Acos(Vector2.Dot(from, to));
            if (theta == 0) return to;

            double sinTheta = Math.Sin(theta);
            return (float)(Math.Sin((1 - alpha) * theta) / sinTheta) * from + (float)(Math.Sin(alpha * theta) / sinTheta) * to;
        }
    }
}
