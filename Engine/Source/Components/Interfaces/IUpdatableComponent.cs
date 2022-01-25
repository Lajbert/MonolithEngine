namespace MonolithEngine
{
    public interface IUpdatableComponent
    {
        public void PreUpdate();

        public void Update();

        public void PostUpdate();
    }
}
