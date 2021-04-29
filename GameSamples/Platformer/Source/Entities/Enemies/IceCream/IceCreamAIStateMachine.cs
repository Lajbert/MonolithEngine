using MonolithEngine;

namespace ForestPlatformerExample
{
    class IceCreamAIStateMachine : AIStateMachine<IceCream>
    {
        public IceCreamAIStateMachine(AIState<IceCream> initialState) : base(initialState)
        {

        }
    }
}
