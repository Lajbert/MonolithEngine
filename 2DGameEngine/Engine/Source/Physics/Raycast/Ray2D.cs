using MonolithEngine.Engine.Source.Entities.Interfaces;
using MonolithEngine.Engine.Source.Graphics.Primitives;
using MonolithEngine.Source;
using MonolithEngine.Source.Util;
using MonolithEngine.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonolithEngine.Engine.Source.Physics.Raycast
{
    public class Ray2D
    {
        public Vector2 Position;
        private Vector2 direction;
        private float angleRad;

        private float x4;
        private float y4;

        private float den;
        private float t;
        private float u;

#if RAYCAST_DEBUG
        public Line debugLine;
        public Circle intersectionMarker;
#endif

        public Ray2D(Vector2 position, Vector2 direction)
        {
            this.Position = position;
            this.direction = direction;
            this.angleRad = MathUtil.RadFromVectors(position, direction);
            this.angleRad = (float)Math.Atan2(direction.Y - position.Y, direction.X - position.X);
#if RAYCAST_DEBUG
            debugLine = new Line(null, position, this.angleRad, 10000f, Color.White, 1);
            intersectionMarker = new Circle(null, position, 10, Color.Red);
#endif
        }

        public Ray2D(Vector2 position, float angleRad = 0f)
        {
            this.Position = position;
            this.angleRad = angleRad;
            this.direction = MathUtil.RadToVector(angleRad);

#if RAYCAST_DEBUG
            debugLine = new Line(null, position, this.angleRad, 10000f, Color.White, 1);
            intersectionMarker = new Circle(null, position, 10, Color.Red);
#endif
        }

        public void Cast((Vector2 from, Vector2 to) target, ref Vector2 result)
        {
            x4 = Position.X + direction.X;
            y4 = Position.Y + direction.Y;

            den = (target.from.X - target.to.X) * (Position.Y - y4) - (target.from.Y - target.to.Y) * (Position.X - x4);
            if (den == 0)
            {
                result.X = result.Y = 0;
                return;
            }

            t = ((target.from.X - Position.X) * (Position.Y - y4) - (target.from.Y - Position.Y) * (Position.X - x4)) / den;
            u = -((target.from.X - target.to.X) * (target.from.Y - Position.Y) - (target.from.Y - target.to.Y) * (target.from.X - Position.X)) / den;
            if (t > 0 && t < 1 && u > 0)
            {
                result.X = target.from.X + t * (target.to.X - target.from.X);
                result.Y = target.from.Y + t * (target.to.Y - target.from.Y);
                return;
            }
            result.X = result.Y = 0;
        }
    }
}
