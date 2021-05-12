using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace MonolithEngine.Experimental
{
    /// <summary>
    /// A class that emits rays to do raycasting.
    /// </summary>
    public class Ray2DEmitter
    {

        private List<Ray2D> rays;
        private Entity owner;
        private Vector2 closestIntersection;
        public float closestDistance;
        private Vector2 intersection = Vector2.Zero;
        private float delay;

        public Dictionary<Entity, Vector2> ClosestIntersections = new Dictionary<Entity, Vector2>();

        public Ray2DEmitter(Entity owner, float startDegree, float endDegree, int step, int delayMs = 0)
        {
            this.owner = owner;
            float start = Math.Min(startDegree, endDegree);
            float end = Math.Max(startDegree, endDegree);
            //owner.RayEmitter = this;
            rays = new List<Ray2D>();
            for (float i = start; i <= end; i+= step)
            {
                rays.Add(new Ray2D(owner.Transform.Position, MathUtil.DegreesToRad(i)));
            }
            this.delay = delayMs;
        }

        /// <summary>
        /// Updating the rays: checking whether the rays are intersecting with other lines on the map.
        /// </summary>
        public void UpdateRays()
        {
            foreach (Ray2D ray in rays)
            {   
                if (delay != 0 && Timer.IsSet("RayCastDelay")) {
                    return;
                }
                ray.Position = owner.Transform.Position;
                closestIntersection.X = closestIntersection.Y = int.MaxValue;
                closestDistance = float.MaxValue;
#if RAYCAST_DEBUG
                ray.debugLine.Reset();
                ray.intersectionMarker.Visible = false;
                ray.debugLine.Transform.Position = owner.Transform.Position;
                ray.debugLine.From = owner.Transform.Position;
#endif
                foreach (Entity e in new List<Entity>())
                //foreach (Entity e in scene.LayerManager.EntityLayer.GetAll())
                {
                    if (!e.BlocksRay || e.Equals(owner))
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
                    if (closestDistance < float.MaxValue)
                    {
                        ClosestIntersections[e] = closestIntersection;
                    } else
                    {
                        if (ClosestIntersections.ContainsKey(e))
                        {
                            ClosestIntersections.Remove(e);
                        }
                    }
                }
#if RAYCAST_DEBUG
                if (closestDistance < float.MaxValue)
                {
                    ray.intersectionMarker.Transform.Position = closestIntersection;
                    ray.intersectionMarker.Visible = true;
                    ray.debugLine.SetEnd(closestIntersection);
                }
#endif
            }
            if (delay != 0)
            {
                Timer.SetTimer("RayCastDelay", delay);
            }   
        }
    }
}
