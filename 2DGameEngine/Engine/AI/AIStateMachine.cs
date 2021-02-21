using GameEngine2D.Engine.Source.Components;
using GameEngine2D.Engine.Source.Entities.Abstract;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.Engine.AI
{
    public class AIStateMachine<T> : IComponent where T : IGameObject
    {
        public Dictionary<Type, State<T>> states = new Dictionary<Type, State<T>>();

        private State<T> currentState = null;

        public float TimeSpentInCurrentState = 0f;

        public bool UniquePerEntity { get; set; }

        public AIStateMachine(State<T> initialState = null)
        {
            UniquePerEntity = true;
            if (initialState != null)
            {
                states.Add(typeof(T), initialState);
            }
        }

        public void AddState(State<T> state)
        {
            states.Add(typeof(T), state);
        }

        public void RemoveState<R>() where R : State<T>
        {
            states.Remove(typeof(R));
        }

        public R GetState<R>() where R : State<T>
        {
            return (R)states[typeof(R)];
        }

        public R ChangeState<R>() where R : State<T>
        {
            Type newState = typeof(T);
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

        public void Update(GameTime gameTime)
        {
            TimeSpentInCurrentState += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            currentState.Update(gameTime);
        }
    }
}
