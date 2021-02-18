using GameEngine2D.Engine.Source.Entities.Abstract;
using GameEngine2D.Engine.Source.Physics.Trigger;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.Engine.Source.Entities.Interfaces
{
    public interface IHasTrigger : IGameObject
    {
        public ICollection<ITrigger> GetTriggers();

        public bool CanFireTriggers { get; set; }

        public void OnEnterTrigger(string triggerTag, IGameObject otherEntity);

        public void OnLeaveTrigger(string triggerTag, IGameObject otherEntity);
    }
}
