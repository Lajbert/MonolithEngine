using System;
using System.Collections.Generic;

namespace MonolithEngine
{
    public class AIStateMachine<T> : IComponent, IUpdatableComponent where T : IGameObject
    {
        public Dictionary<Type, AIState<T>> states = new Dictionary<Type, AIState<T>>();

        private AIState<T> currentState = null;

        public float TimeSpentInCurrentState = 0f;

        public bool UniquePerEntity { get; set; }

        public bool Enabled = true;

        public AIStateMachine(AIState<T> initialState = null)
        {
            UniquePerEntity = true;
            if (initialState != null)
            {
                states.Add(initialState.GetType(), initialState);
                currentState = initialState;
                currentState.Begin();
            }
        }

        public void AddState(AIState<T> state)
        {
            states.Add(state.GetType(), state);
        }

        public void RemoveState<R>() where R : AIState<T>
        {
            states.Remove(typeof(R));
        }

        public R GetState<R>() where R : AIState<T>
        {
            return (R)states[typeof(R)];
        }

        public R ChangeState<R>() where R : AIState<T>
        {
            Type newState = typeof(R);
            if (currentState.GetType().Equals(newState))
            {
                return (R)currentState;
            }
            
            if (currentState != null)
            {
                currentState.End();
            }

            TimeSpentInCurrentState = 0f;
            currentState = states[newState];
            currentState.Begin();
            return (R)currentState;
        }

        public void Update()
        {
            if (!Enabled || currentState == null)
            {
                return;
            }
            TimeSpentInCurrentState += Globals.FixedUpdateMultiplier;
            currentState.FixedUpdate();
        }

        public AIState<T> GetCurrentState()
        {
            return currentState;
        }
    }
}
