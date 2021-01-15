using GameEngine2D.Entities;
using GameEngine2D.Source;
using GameEngine2D.Source.Util;
using GameEngine2D.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.Engine.Source.Physics.Raycast
{
    public class Ray2DEmitter
    {

        private List<Ray2D> rays;
        private Entity owner;
        private Vector2 closestIntersection;
        public float closestDistance;
        private Vector2 intersection = Vector2.Zero;

        public Ray2DEmitter(Entity owner)
        {
            this.owner = owner;
            owner.RayEmitter = this;
            rays = new List<Ray2D>();
            for (float i = 0; i <= 360; i+=1f)
            {
                rays.Add(new Ray2D(owner.Position, MathUtil.DegreesToRad(i)));
            }
        }
        public void UpdateRays()
        {
            foreach (Ray2D ray in rays)
            {
                ray.Position = owner.Position;
                closestIntersection.X = closestIntersection.Y = int.MaxValue;
                closestDistance = float.MaxValue;
#if RAYCAST_DEBUG
                ray.debugLine.Reset();
                ray.intersectionMarker.Visible = false;
                ray.debugLine.Position = owner.Position;
                ray.debugLine.From = owner.Position;
#endif
                foreach (Entity e in LayerManager.Instance.RayBlockersLayer.GetAll())
                {
                    if (!e.BlocksRay)
                    {
                        continue;
                    }
                    foreach ((Vector2, Vector2) line in e.GetRayBlockerLines())
                    {
                        ray.Cast(line, ref intersection);
#if RAYCAST_DEBUG
                        if (intersection != Vector2.Zero)
                        {
                            float distance = Vector2.Distance(ray.Position, intersection);
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
                    ray.intersectionMarker.Position = closestIntersection;
                    ray.intersectionMarker.Visible = true;
                    ray.debugLine.SetEnd(closestIntersection);
                }
#endif
            }
        }
    }
}
