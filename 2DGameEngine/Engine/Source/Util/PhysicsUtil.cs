using GameEngine2D.Engine.Source.Physics.Interface;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.Engine.Source.Util
{
    public class PhysicsUtil
    {
        public static void ApplyRepel(IColliderEntity thisCollider, IColliderEntity otherCollider, float repelForceOverride = 0, RepelMode repelMode = RepelMode.BOTH)
        {
            float angle = (float)Math.Atan2(otherCollider.GetCollisionComponent().Position.Y - thisCollider.GetCollisionComponent().Position.Y, otherCollider.GetCollisionComponent().Position.X - thisCollider.GetCollisionComponent().Position.X);
            float repelForce;
            if (repelForceOverride == 0)
            {
                repelForce = Vector2.Distance(thisCollider.GetCollisionComponent().Position, otherCollider.GetCollisionComponent().Position);
            } else
            {
                repelForce = repelForceOverride;
            }
            if (repelMode == RepelMode.ONLY_THIS || repelMode == RepelMode.BOTH)
            {
                (thisCollider as PhysicalEntity).AddForce(new Vector2((float)-Math.Cos(angle) * repelForce, (float)-Math.Sin(angle) * repelForce));
            }
            if (repelMode == RepelMode.OTHER_COLLIDER_ONLY || repelMode == RepelMode.BOTH)
            {
                (otherCollider as PhysicalEntity).AddForce(new Vector2((float)Math.Cos(angle) * repelForce, (float)Math.Sin(angle) * repelForce));
            }
        }
    }

    public enum RepelMode
    {
        BOTH,
        ONLY_THIS,
        OTHER_COLLIDER_ONLY
    }
}
