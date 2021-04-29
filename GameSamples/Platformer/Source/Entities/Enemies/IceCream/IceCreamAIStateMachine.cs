using MonolithEngine.Engine.AI;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForestPlatformerExample.Source.Entities.Enemies.IceCream
{
    class IceCreamAIStateMachine : AIStateMachine<IceCream>
    {
        public IceCreamAIStateMachine(AIState<IceCream> initialState) : base(initialState)
        {

        }
    }
}
