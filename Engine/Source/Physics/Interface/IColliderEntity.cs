using System;
using System.Collections.Generic;

namespace MonolithEngine
{
    /// <summary>
    /// Interface representing that entity is a collider
    /// </summary>
    public interface IColliderEntity : IHasTrigger
    {

        public bool CollisionsEnabled { get; set; }

        public ICollisionComponent GetCollisionComponent();

        internal void CollisionStarted(IGameObject otherCollider, bool allowOverlap);

        internal void CollisionEnded(IGameObject otherCollider);

        internal Dictionary<Type, bool> GetCollidesAgainst();

        internal HashSet<Type> GetTriggeredAgainst();

        public bool CheckGridCollisions { get; set; }
    }
}
