using MonolithEngine.Engine.Source.Entities.Abstract;
using MonolithEngine.Engine.Source.Physics.Trigger;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonolithEngine.Engine.Source.Entities.Interfaces
{
    public interface IHasTrigger : IGameObject
    {
        public bool IsDestroyed { get; }

        public ICollection<ITrigger> GetTriggers();

        public bool CanFireTriggers { get; set; }

        public void OnEnterTrigger(string triggerTag, IGameObject otherEntity);

        public void OnLeaveTrigger(string triggerTag, IGameObject otherEntity);
    }
}
