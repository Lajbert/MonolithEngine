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
        private Vector2 position;
        public Vector2 Position { 
            get => position + owner.Position;
            set => position = value;
        }
        public float Radius;

        private Entity owner;

        public CircleCollider(Entity owner, float radius, Vector2? positionOffset = null)
        {
            this.owner = owner;
            Radius = radius;
            this.Position = positionOffset.HasValue ? positionOffset.Value : Vector2.Zero;
        }

        private float maxDistance;
        private float distSqrt;
        private float intersection;
        public (bool, float) CollidesWith(ICircleCollider otherCollider)
        {
            maxDistance = Radius * otherCollider.CircleCollider.Radius;
            distSqrt = Vector2.DistanceSquared(Position, otherCollider.CircleCollider.Position);
            intersection = (Radius + otherCollider.CircleCollider.Radius - distSqrt) / (Radius + otherCollider.CircleCollider.Radius);
            return (distSqrt < maxDistance * maxDistance, intersection);
        }
    }
}
