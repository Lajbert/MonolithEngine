using Microsoft.Xna.Framework;
using MonolithEngine;

namespace ForestPlatformerExample
{
    class CarrotIdleState : AIState<Carrot>
    {
        public CarrotIdleState(Carrot carrot) : base(carrot)
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
            
        }
    }
}
