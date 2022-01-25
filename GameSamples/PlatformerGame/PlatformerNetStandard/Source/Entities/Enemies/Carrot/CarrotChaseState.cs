namespace ForestPlatformerExample
{
    class CarrotChaseState : CarrotPatrolState
    {
        public CarrotChaseState(Carrot carrot) : base (carrot)
        {

        }

        public override void Begin()
        {
            controlledEntity.CurrentSpeed = controlledEntity.DefaultSpeed;
            checkCollisions = false;
        }
    }
}
