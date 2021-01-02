using GameEngine2D.Engine.src.Graphics.Primitives;
using GameEngine2D.src;
using GameEngine2D.src.Util;
using GameEngine2D.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.Engine.src.Physics.Raycast
{
    class Ray2D
    {
        public Vector2 position;
        public Vector2 direction;
        public float angleRad;

        public Ray2D(Vector2 position, Vector2 direction)
        {
            this.position = position;
            this.direction = direction;
            this.angleRad = (float)Math.Atan2(direction.Y - position.Y, direction.X - position.X);
        }

        public Ray2D(Vector2 position, float angleRad = 0f)
        {
            this.position = position;
            this.angleRad = angleRad;
            this.direction = MathUtil.RadToVector(angleRad);

#if RAYCAST_DEBUG
            new Line(Scene.Instance.GetEntityLayer(), null, position, this.angleRad, 1000f, Color.Blue);
#endif
        }

        public Vector2 Cast(Line target)
        {
            float x1 = target.from.X;
            float y1 = target.from.Y;
            float x2 = target.to.X;
            float y2 = target.to.Y;

            float x3 = position.X;
            float y3 = position.Y;
            float x4 = position.X + direction.X;
            float y4 = position.Y + direction.Y;

            float den = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);
            if (den == 0)
            {
                return Vector2.Zero;
            }

            float t = ((x1 - x3) * (y3 - y4) - (y1 - y3) * (x3 - x4)) / den;
            float u = -((x1 - x2) * (y1 - y3) - (y1 - y2) * (x1 - x3)) / den;
            if (t > 0 && t < 1 && u > 0)
            {
                return new Vector2(x1 + t * (x2 - x1), y1 + t * (y2 - y1));
            }
            return Vector2.Zero;
        }
    }
}
