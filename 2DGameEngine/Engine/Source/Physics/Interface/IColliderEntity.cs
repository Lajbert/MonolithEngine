using GameEngine2D.Engine.Source.Physics.Collision;
using GameEngine2D.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.Engine.Source.Physics.Interface
{
    public interface IColliderEntity : ICircleCollider
    {
        public HashSet<CollisionType> GetCollisionProfile();

        public ICollection<string> GetTags();

        public Vector2 GetPosition();

        public void SetPosition(Vector2 position);

        public CircleCollisionComponent GetCircleCollisionComponent();

        public void OnCollisionStart(IColliderEntity otherCollider);

        public void OnCollisionEnd(IColliderEntity otherCollider);
        public HashSet<string> GetCollidesAgainst();
    }
}
