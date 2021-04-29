using System.Collections.Generic;

namespace MonolithEngine
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
