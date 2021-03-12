using ForestPlatformerExample.Source.Enemies;
using ForestPlatformerExample.Source.Entities.Enemies.CarrotAI;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForestPlatformerExample.Source.Entities.Enemies.CarrotAI
{
    class CarrotChaseState : CarrotPatrolState
    {
        public CarrotChaseState(Carrot carrot) : base (carrot)
        {

        }

        public override void Begin()
        {
            checkCollisions = false;
        }
    }
}
