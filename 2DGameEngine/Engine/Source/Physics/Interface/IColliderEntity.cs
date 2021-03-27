using MonolithEngine.Engine.Source.Entities.Abstract;
using MonolithEngine.Engine.Source.Entities.Interfaces;
using MonolithEngine.Engine.Source.Entities.Transform;
using MonolithEngine.Engine.Source.Physics.Collision;
using MonolithEngine.Engine.Source.Physics.Trigger;
using MonolithEngine.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonolithEngine.Engine.Source.Physics.Interface
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
