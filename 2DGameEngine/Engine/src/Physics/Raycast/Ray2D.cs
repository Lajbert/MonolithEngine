using GameEngine2D.Engine.src.Graphics.Primitives;
using GameEngine2D.src;
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
        public float angle;

        public Ray2D(Vector2 position, Vector2 direction) : this(position, (float)Math.Atan2(direction.Y - position.Y, direction.X - position.X))
        {
        }

        public Ray2D(Vector2 position, float angle = 0f)
        {
            this.position = position;
            this.angle = angle;

#if RAYCAST_DEBUG
            new Line(Scene.Instance.GetEntityLayer(), null, position, this.angle, 1000f, Color.Blue);
#endif
        }

        public Vector2 Cast(Line target)
        {
            return Vector2.Zero;
        }
    }
}
