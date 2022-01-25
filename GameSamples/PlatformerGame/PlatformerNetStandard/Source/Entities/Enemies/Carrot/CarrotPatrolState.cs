using MonolithEngine;

namespace ForestPlatformerExample
{
    class CarrotPatrolState : AIState<Carrot>
    {

        protected bool checkCollisions;

        public CarrotPatrolState(Carrot carrot) : base(carrot)
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

            AIUtil.Patrol(checkCollisions, controlledEntity);
        }

    }
}
