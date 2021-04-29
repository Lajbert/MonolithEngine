using Microsoft.Xna.Framework;
using MonolithEngine;

namespace ForestPlatformerExample
{
    class IceCreamAttackState : AIState<IceCream>
    {
        public IceCreamAttackState(IceCream iceCream) : base(iceCream)
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
            }
            else
            {
                controlledEntity.CurrentFaceDirection = Direction.EAST;
            }

            controlledEntity.Attack();
        }
    }
}