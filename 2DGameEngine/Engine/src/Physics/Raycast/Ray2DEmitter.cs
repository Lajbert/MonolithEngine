using GameEngine2D.Entities;
using GameEngine2D.src;
using GameEngine2D.src.Util;
using GameEngine2D.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.Engine.src.Physics.Raycast
{
    class Ray2DEmitter
    {

        private List<Ray2D> rays;
        private Entity owner;
        private Vector2 closestIntersection;
        public float closestDistance;

        public Ray2DEmitter(Entity owner)
        {
            this.owner = owner;
            owner.SetRayEmitter(this);
            rays = new List<Ray2D>();
            for (float i = 0; i <= 360; i+=1f)
            {
                rays.Add(new Ray2D(owner.GetPosition(), MathUtil.DegreesToRad(i)));
            }
        }
        public void UpdateRays()
        {
            foreach (Ray2D ray in rays)
            {
                ray.position = owner.GetPosition();
                closestIntersection = new Vector2(int.MaxValue, int.MaxValue);
                closestDistance = float.MaxValue;
#if RAYCAST_DEBUG
                ray.debugLine.Reset();
                ray.debugLine.SetPosition(owner.GetPosition());
                ray.debugLine.from = owner.GetPosition();
#endif
                foreach (Entity e in Scene.Instance.GetRayBlockersLayer().GetAll())
                {
                    if (!e.BlocksRay())
                    {
                        continue;
                    }
                    foreach ((Vector2, Vector2) line in e.GetRayBlockerLines())
                    {
                        Vector2 intersection = ray.Cast(line);
#if RAYCAST_DEBUG
                        if (intersection != Vector2.Zero)
                        {
                            float distance = Vector2.Distance(ray.position, intersection);
                            if (distance < closestDistance)
                            {
                                closestDistance = distance;
                                closestIntersection = intersection;
                            }
                        }
#endif
                    }
                }
#if RAYCAST_DEBUG
                if (closestDistance < float.MaxValue)
                {
                    //ray.intersectionMarker.SetPosition(closestIntersection);
                    ray.debugLine.SetEnd(closestIntersection);
                }
#endif
            }
        }
    }
}
