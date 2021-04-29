using Microsoft.Xna.Framework;
using MonolithEngine;

namespace ForestPlatformerExample
{
    class TrunkAttackState : AIState<Trunk>
    {
        public TrunkAttackState(Trunk trunk) : base(trunk)
        {

        }

        public override void Begin()
        {
            controlledEntity.Velocity = Vector2.Zero;
        }

        public override void End()
        {
            
        }

        public override void FixedUpdate()
        {
            if (controlledEntity.Target == null)
            {
                return;
            }

            if (controlledEntity.Target.Transform.X < controlledEntity.Transform.X)
            {
                controlledEntity.CurrentFaceDirection = Direction.WEST;
            } else
            {
                controlledEntity.CurrentFaceDirection = Direction.EAST;
            }

            controlledEntity.Shoot();
        }
    }
}
