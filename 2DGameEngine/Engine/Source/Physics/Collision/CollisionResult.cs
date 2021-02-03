using GameEngine2D.Engine.Source.Entities.Interfaces;
using GameEngine2D.Engine.Source.Physics.Interface;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.Engine.Source.Physics.Collision
{
    public struct CollisionResult
    {
        public static CollisionResult NO_COLLISION { get; }

        private ICircleCollider thisCollider;
        private ICircleCollider otherCollider;
        private float distance;

        public CollisionResult(ICircleCollider thisCollider, ICircleCollider otherCollider, float distance)
        {
            this.thisCollider = thisCollider;
            this.otherCollider = otherCollider;
            this.distance = distance;
        }

        public float GetRepelForce()
        {
            return(thisCollider.CircleCollider.Radius + otherCollider.CircleCollider.Radius - distance) / (thisCollider.CircleCollider.Radius + otherCollider.CircleCollider.Radius);
        }

        public void ApplyRepel(float repelForceOverride = 0, RepelMode repelMode = RepelMode.BOTH)
        {
            if (!(thisCollider is IHasCircleCollisionPhysics) || !(otherCollider is IHasCircleCollisionPhysics)) {
                throw new Exception("Can't apply repel force to these objects!");
            }
            float angle = (float)Math.Atan2(otherCollider.CircleCollider.Position.Y - thisCollider.CircleCollider.Position.Y, otherCollider.CircleCollider.Position.X - thisCollider.CircleCollider.Position.X);
            float repelForce = repelForceOverride == 0 ? distance : repelForceOverride;
            if (repelMode == RepelMode.ONLY_THIS || repelMode == RepelMode.BOTH)
            {
                (thisCollider as IHasCircleCollisionPhysics).AddForce(new Vector2((float)-Math.Cos(angle) * repelForce, (float)-Math.Sin(angle) * repelForce));
            }
            if (repelMode == RepelMode.OTHER_COLLIDER_ONLY || repelMode == RepelMode.BOTH)
            {
                (otherCollider as IHasCircleCollisionPhysics).AddForce(new Vector2((float)Math.Cos(angle) * repelForce, (float)Math.Sin(angle) * repelForce));
            }
            
        }

        public static bool operator ==(CollisionResult a, CollisionResult b)
        {
            return a.distance == b.distance &&
                   EqualityComparer<ICircleCollider>.Default.Equals(a.thisCollider, b.thisCollider) &&
                   EqualityComparer<ICircleCollider>.Default.Equals(a.otherCollider, b.otherCollider);
        }

        public static bool operator !=(CollisionResult a, CollisionResult b)
        {
            return !(a == b);
        }

        public enum RepelMode
        {
            BOTH,
            ONLY_THIS,
            OTHER_COLLIDER_ONLY
        }
    }
}
