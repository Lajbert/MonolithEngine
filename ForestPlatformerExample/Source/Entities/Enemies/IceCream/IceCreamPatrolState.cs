using MonolithEngine.Engine.AI;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForestPlatformerExample.Source.Entities.Enemies.IceCream
{
    class IceCreamPatrolState : AIState<IceCream>
    {

        protected bool checkCollisions;

        public IceCreamPatrolState(IceCream carrot) : base(carrot)
        {
        }

        public override void Begin()
        {
            checkCollisions = true;
            controlledEntity.CurrentSpeed = controlledEntity.DefaultSpeed;
        }

        public override void End()
        {

        }

        public override void FixedUpdate()
        {

            AIUtil.Patrol(checkCollisions, controlledEntity, 5000);
        }

    }
}
