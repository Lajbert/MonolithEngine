using ForestPlatformerExample.Source.Enemies;
using MonolithEngine.Engine.AI;
using MonolithEngine.Engine.Source.Entities;
using MonolithEngine.Engine.Source.Global;
using MonolithEngine.Engine.Source.Physics.Collision;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForestPlatformerExample.Source.Entities.Enemies.CarrotAI
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
