using GameEngine2D.Engine.Source.Entities.Abstract;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.Engine.AI
{
    public abstract class AIState<T> where T : IGameObject
    {
        protected T controlledEntity;

        public AIState(T controlledEntity)
        {
            this.controlledEntity = controlledEntity;
        }

        public virtual void Begin()
        {

        }

        public virtual void Update(GameTime gameTime)
        {

        }

        public virtual void End()
        {

        }

    }
}
