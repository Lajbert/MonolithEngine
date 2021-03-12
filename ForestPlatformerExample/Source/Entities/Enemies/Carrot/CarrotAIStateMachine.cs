using ForestPlatformerExample.Source.Enemies;
using GameEngine2D.Engine.AI;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForestPlatformerExample.Source.Entities.Enemies.CarrotAI
{
    class CarrotAIStateMachine : AIStateMachine<Carrot>
    {
        public CarrotAIStateMachine(AIState<Carrot> initialState) : base(initialState)
        {

        }
    }
}
