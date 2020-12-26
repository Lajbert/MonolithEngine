using System;
using System.Collections.Generic;
using System.Text;
using _2DGameEngine.Entities;
using _2DGameEngine.Entities.Interfaces;
using _2DGameEngine.Global;
using _2DGameEngine.Util;
using Microsoft.Xna.Framework;

namespace _2DGameEngine.src.Camera
{
    class Camera
    {

        private Entity target;
        private Vector2 position;
        private Vector2 offset;
        private Vector2 direction;

        public void Follow(Entity entity, bool immediate, Vector2 offset)
        {
            target = entity;
            this.offset = offset;
            if (immediate)
            {
                Recenter();
            }
        }

        public void Follow(Entity entity, bool immediate)
        {
            Follow(entity, immediate, new Vector2(0, 0));
        }

        private void Recenter()
        {
            if (target != null)
            {
                position = target.GetGridCoord() + offset;
            }
        }

        public void UpdateCamera(GameTime gameTime)
        {
            float tmod = (float)gameTime.ElapsedGameTime.TotalSeconds * Constants.TIME_OFFSET;
            if (target != null)
            {
                float s = 0.006f;
                float deadZone = 5;
                //float s = 1f;
                //float deadZone = 5f;
                Vector2 tx = target.GetGridCoord() + offset;

                float d = Dist(position, tx);
                Logger.Log("D: " + d);
                Logger.Log("DeadZone: " + deadZone);
                if (d >= deadZone)
                {
                    float a = (float)Math.Atan2(tx.Y - position.Y, tx.X - position.X);
                    direction.X -= (float)Math.Cos(a) * (d - deadZone) * s * tmod;
                    direction.Y -= (float)Math.Sin(a) * (d - deadZone) * s * tmod;
                }

                float frict = 0.89f;
                position.X += direction.X * tmod;
                direction.X *= (float)Math.Pow(frict, tmod);

                position.Y += direction.Y * tmod;
                direction.Y *= (float)Math.Pow(frict, tmod);

                RootContainer.Instance.SetPosition(position * 0.5f);

                // Rounding
                //float x = (float)Math.Round(RootContainer.Instance.GetRootPosition().X);
                //float y = (float)Math.Round(RootContainer.Instance.GetRootPosition().Y);
                //RootContainer.Instance.SetPosition(new Vector2(x, y));

            }
        }

        public float Dist(Vector2 a, Vector2 b)
        {
            return (float)Math.Sqrt(DistSqr(a, b));
        }

        public float DistSqr(Vector2 a, Vector2 b)
        {
            return (a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y);
        }

        /*public float DistSqr(float ax, float ay, float bx, float by)
        {
            return (ax - bx) * (ax - bx) + (ay - by) * (ay - by);
        }*/
    }
}
