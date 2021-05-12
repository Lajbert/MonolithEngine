namespace MonolithEngine
{
    /// <summary>
    /// Interface for triggers.
    /// </summary>
    public interface ITrigger : IComponent
    {
        public bool IsInsideTrigger(IGameObject otherObject);

        public string GetTag();
    }
}
