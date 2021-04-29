using MonolithEngine.Engine.AI;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForestPlatformerExample.Source.Entities.Enemies.Trunk
{
    class TrunkAIStateMachine : AIStateMachine<Trunk>
    {
        public TrunkAIStateMachine(AIState<Trunk> initialState) : base(initialState)
        {

        }
    }
}
