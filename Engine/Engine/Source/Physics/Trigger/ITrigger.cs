namespace MonolithEngine
{
    public interface ITrigger : IComponent
    {
        public bool IsInsideTrigger(IGameObject otherObject);

        public string GetTag();
    }
}
