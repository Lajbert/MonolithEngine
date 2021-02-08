using GameEngine2D.Engine.Source.Physics.Collision;
using GameEngine2D.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.Engine.Source.Physics.Interface
{
    public interface IPhysicsEntity : ICircleCollider
    {
        public HashSet<CollisionType> GetCollisionProfile();

        public ICollection<string> GetTags();

        public Vector2 GetPosition();

        public void SetPosition(Vector2 position);

        public Vector2 GetVelocity();

        public void SetVelocity(Vector2 velocity);

        public void AddVelocity(Vector2 velocity);

        public CircleCollisionComponent GetCircleCollisionComponent();

        public void OnCollisionStart(IPhysicsEntity otherCollider);

        public void OnCollisionEnd(IPhysicsEntity otherCollider);
        public HashSet<string> GetCollidesAgainst();
    }
}
