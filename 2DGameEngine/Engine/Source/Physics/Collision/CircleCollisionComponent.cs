using GameEngine2D.Engine.Source.Physics.Interface;
using GameEngine2D.Entities;
using GameEngine2D.Global;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.Engine.Source.Physics.Collision
{
    public class CircleCollisionComponent
    {
        private Vector2 positionOffset;
        public Vector2 Position {
            get => positionOffset + owner.GetPosition();
            private set { }
        }
        public float Radius;

        private ICircleCollider owner;

        public bool IsCircleCollider = true;

        public CircleCollisionComponent(ICircleCollider owner, float radius, Vector2? positionOffset = null)
        {
            this.owner = owner;
            Radius = radius;
            this.positionOffset = positionOffset.HasValue ? positionOffset.Value : Vector2.Zero;
        }

        private float maxDistance;
        private float distance;
        //private float intersection;
        /*public CollisionResult Overlaps(ICircleCollider otherCollider)
        {
            maxDistance = Radius + otherCollider.CircleCollider.Radius;
            distance = Vector2.Distance(Position, otherCollider.CircleCollider.Position);
            intersection = (Radius + otherCollider.CircleCollider.Radius - distance) / (Radius + otherCollider.CircleCollider.Radius);
            bool overlaps = distance <= maxDistance;
            if (overlaps)
            {
                overlaps = true;
            }
            return distance <= maxDistance ? new CollisionResult(owner, otherCollider, distance) : CollisionResult.NO_COLLISION;
        }*/

        public bool Overlaps(ICircleCollider otherCollider)
        {
            //TODO: review if this fast check is needed
            if ((Math.Abs(Position.X - otherCollider.CircleCollider.Position.X) > Config.GRID * 2 && Math.Abs(Position.Y - otherCollider.CircleCollider.Position.Y) > Config.GRID * 2))
            {
                return false;
            }
            maxDistance = Radius + otherCollider.CircleCollider.Radius;
            distance = Vector2.Distance(Position, otherCollider.CircleCollider.Position);
            //repelForce = (Radius + otherCollider.CircleCollider.Radius - distance) / (Radius + otherCollider.CircleCollider.Radius);
            return distance <= maxDistance;
        }
    }
}
