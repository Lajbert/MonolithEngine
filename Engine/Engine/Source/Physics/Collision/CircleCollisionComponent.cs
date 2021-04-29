using MonolithEngine.Engine.Source.Physics.Interface;
using MonolithEngine.Entities;
using MonolithEngine.Global;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonolithEngine.Engine.Source.Physics.Collision
{
    public class CircleCollisionComponent : AbstractCollisionComponent
    {
        public float Radius;

        public bool IsCircleCollider = true;

        public CircleCollisionComponent(IColliderEntity owner, float radius, Vector2 positionOffset = default) : base (ColliderType.CIRCLE, owner, positionOffset)
        {
            Radius = radius;
        }

        private float maxDistance;
        private float distance;

        public override bool CollidesWith(IColliderEntity otherCollider)
        {
            if (otherCollider.GetCollisionComponent().GetType() == ColliderType.CIRCLE)
            {
                //TODO: review if this fast check is needed
                CircleCollisionComponent other = otherCollider.GetCollisionComponent() as CircleCollisionComponent;
                if ((Math.Abs(Position.X - otherCollider.GetCollisionComponent().Position.X) > Config.GRID * 2 && Math.Abs(Position.Y - other.Position.Y) > Config.GRID * 2))
                {
                    return false;
                }
                maxDistance = Radius + other.Radius;
                distance = Vector2.Distance(Position, other.Position);
                return distance <= maxDistance;
            }
            else if (otherCollider.GetCollisionComponent().GetType() == ColliderType.BOX)
            {
                BoxCollisionComponent box = otherCollider.GetCollisionComponent() as BoxCollisionComponent;
                Vector2 closestPoint = new Vector2(Math.Clamp(Position.X, box.Position.X, box.Position.X + box.Width), Math.Clamp(Position.Y, box.Position.Y, box.Position.Y + box.Height));
                return Vector2.DistanceSquared(Position, closestPoint) < Radius * Radius;
            }
            throw new Exception("Unknown collider type");
        }

#if DEBUG
        protected override void CreateDebugVisual()
        {
            
        }
#endif
    }
}
