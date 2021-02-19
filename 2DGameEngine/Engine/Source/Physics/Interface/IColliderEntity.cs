using GameEngine2D.Engine.Source.Entities.Abstract;
using GameEngine2D.Engine.Source.Entities.Interfaces;
using GameEngine2D.Engine.Source.Entities.Transform;
using GameEngine2D.Engine.Source.Physics.Collision;
using GameEngine2D.Engine.Source.Physics.Trigger;
using GameEngine2D.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.Engine.Source.Physics.Interface
{
    public interface IColliderEntity : IHasTrigger
    {

        public bool CollisionsEnabled { get; set; }

        public ICollisionComponent GetCollisionComponent();

        public void OnCollisionStart(IGameObject otherCollider);

        public void OnCollisionEnd(IGameObject otherCollider);

        public HashSet<string> GetCollidesAgainst();

        public bool CheckGridCollisions { get; set; }
    }
}
