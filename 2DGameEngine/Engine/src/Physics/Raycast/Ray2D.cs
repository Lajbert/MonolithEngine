using GameEngine2D.Engine.src.Entities.Interfaces;
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

        private float x4;
        private float y4;

        private float den;
        float t;
        float u;

#if RAYCAST_DEBUG
        public Line debugLine;
        public Circle intersectionMarker;
#endif

        public Ray2D(Vector2 position, Vector2 direction)
        {
            this.position = position;
            this.direction = direction;
            this.angleRad = MathUtil.AngleFromVectors(position, direction);
            this.angleRad = (float)Math.Atan2(direction.Y - position.Y, direction.X - position.X);
#if RAYCAST_DEBUG
            debugLine = new Line(null, position, this.angleRad, 10000f, Color.White, 1);
            intersectionMarker = new Circle(null, position, 10, Color.Red);
#endif
        }

        public Ray2D(Vector2 position, float angleRad = 0f)
        {
            this.position = position;
            this.angleRad = angleRad;
            this.direction = MathUtil.RadToVector(angleRad);

#if RAYCAST_DEBUG
            debugLine = new Line(null, position, this.angleRad, 10000f, Color.White, 1);
            intersectionMarker = new Circle(null, position, 10, Color.Red);
#endif
        }

        public Vector2 Cast((Vector2 from, Vector2 to) target)
        {
            x4 = position.X + direction.X;
            y4 = position.Y + direction.Y;

            den = (target.from.X - target.to.X) * (position.Y - y4) - (target.from.Y - target.to.Y) * (position.X - x4);
            if (den == 0)
            {
                return Vector2.Zero;
            }

            t = ((target.from.X - position.X) * (position.Y - y4) - (target.from.Y - position.Y) * (position.X - x4)) / den;
            u = -((target.from.X - target.to.X) * (target.from.Y - position.Y) - (target.from.Y - target.to.Y) * (target.from.X - position.X)) / den;
            if (t > 0 && t < 1 && u > 0)
            {
                return new Vector2(target.from.X + t * (target.to.X - target.from.X), target.from.Y + t * (target.to.Y - target.from.Y));
            }
            return Vector2.Zero;
        }
    }
}
