using GameEngine2D.Engine.Source.Physics.Interface;
using GameEngine2D.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.Engine.Source.Physics.Collision
{
    public class CircleCollider
    {
        private Vector2 positionOffset;
        public Vector2 Position {
            get => positionOffset + owner.Position ;
            private set { }
        }
        public float Radius;

        private Entity owner;

        public CircleCollider(Entity owner, float radius, Vector2? positionOffset = null)
        {
            this.owner = owner;
            Radius = radius;
            this.positionOffset = positionOffset.HasValue ? positionOffset.Value : Vector2.Zero;
        }

        private float maxDistance;
        private float distance;
        private float intersection;
        public (bool, float) Overlaps(ICircleCollider otherCollider)
        {
            maxDistance = Radius + otherCollider.CircleCollider.Radius;
            distance = Vector2.Distance(Position, otherCollider.CircleCollider.Position);
            /*distSqrt = (otherCollider.CircleCollider.Position.X - Position.X) * (otherCollider.CircleCollider.Position.X - Position.X) 
                + (otherCollider.CircleCollider.Position.Y - Position.Y) * (otherCollider.CircleCollider.Position.Y - Position.Y);*/
            intersection = (Radius + otherCollider.CircleCollider.Radius - distance) / (Radius + otherCollider.CircleCollider.Radius);
            bool overlaps = distance <= maxDistance;
            if (overlaps)
            {
                overlaps = true;
            }
            return (distance <= maxDistance, intersection);
        }
    }
}
