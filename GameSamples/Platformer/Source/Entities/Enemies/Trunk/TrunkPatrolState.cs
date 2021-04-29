using MonolithEngine.Engine.AI;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForestPlatformerExample.Source.Entities.Enemies.Trunk
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
