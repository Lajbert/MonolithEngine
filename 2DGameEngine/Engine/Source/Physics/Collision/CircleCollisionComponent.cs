using GameEngine2D.Engine.Source.Physics.Interface;
using GameEngine2D.Entities;
using GameEngine2D.Global;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.Engine.Source.Physics.Collision
{
    public class CircleCollisionComponent : AbstractCollisionComponent
    {
        public float Radius;

        public bool IsCircleCollider = true;

        public CircleCollisionComponent(IColliderEntity owner, float radius, Vector2? positionOffset = null) : base (owner, positionOffset)
        {
            Radius = radius;
        }

        private float maxDistance;
        private float distance;

        public override bool Overlaps(IColliderEntity otherCollider)
        {
            if (otherCollider.GetCollisionComponent() is CircleCollisionComponent)
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
            return false;
        }
    }
}
