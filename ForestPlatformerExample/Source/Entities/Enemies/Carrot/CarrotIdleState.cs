using ForestPlatformerExample.Source.Enemies;
using MonolithEngine.Engine.AI;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForestPlatformerExample.Source.Entities.Enemies.CarrotAI
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
    }
}
