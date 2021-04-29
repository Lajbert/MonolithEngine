using System.Collections.Generic;

namespace MonolithEngine
{
    public interface IColliderEntity : IHasTrigger
    {

        public bool CollisionsEnabled { get; set; }

        public ICollisionComponent GetCollisionComponent();

        internal void CollisionStarted(IGameObject otherCollider, bool allowOverlap);

        internal void CollisionEnded(IGameObject otherCollider);

        public Dictionary<string, bool> GetCollidesAgainst();

        public bool CheckGridCollisions { get; set; }
    }
}
