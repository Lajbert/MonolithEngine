namespace MonolithEngine
{
    /// <summary>
    /// Represents a given AI state.
    /// </summary>
    /// <typeparam name="T">The entity the state controls</typeparam>
    public abstract class AIState<T> where T : IGameObject
    {
        protected T controlledEntity;

        public AIState(T controlledEntity)
        {
            this.controlledEntity = controlledEntity;
        }

        public abstract void Begin();

        public abstract void FixedUpdate();

        public abstract void End();

    }
}
