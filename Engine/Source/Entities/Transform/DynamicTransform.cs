using Microsoft.Xna.Framework;

namespace MonolithEngine
{
    /// <summary>
    /// A Transform that has Velocity
    /// </summary>
    class DynamicTransform : StaticTransform
    {

        PhysicalEntity entity;

        public DynamicTransform(IGameObject owner, Vector2 position = default) : base(owner, position)
        {
            entity = owner as PhysicalEntity;
        }

        private Vector2 velocity;

        public override Vector2 Velocity
        {
            get
            {
                if (entity.MountedOn == null)
                {
                    return velocity;
                }
                return entity.MountedOn.Transform.Velocity + velocity;
            }

            set => velocity= value;
        }

        public override float VelocityX
        {
            get
            {
                return velocity.X;
            }

            set => velocity.X = value;
        }

        public override float VelocityY
        {
            get
            {
                return velocity.Y;
            }

            set => velocity.Y = value;
        }
    }
}
