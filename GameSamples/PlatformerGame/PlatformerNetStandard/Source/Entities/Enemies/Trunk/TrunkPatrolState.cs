using MonolithEngine;

namespace ForestPlatformerExample
{
    class TrunkPatrolState : AIState<Trunk>
    {
        public TrunkPatrolState(Trunk trunk) : base (trunk)
        {

        }

        public override void Begin()
        {
            
        }

        public override void End()
        {
            
        }

        public override void FixedUpdate()
        {
            if (controlledEntity.IsAttacking)
            {
                return;
            }
            AIUtil.Patrol(true, controlledEntity);
        }
    }
}
