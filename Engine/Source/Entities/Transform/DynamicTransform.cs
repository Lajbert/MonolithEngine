using Microsoft.Xna.Framework;
using System;

namespace MonolithEngine
{
    /// <summary>
    /// A Transform that has Velocity
    /// </summary>
    class DynamicTransform : StaticTransform
    {

        private PhysicalEntity entity;

        public DynamicTransform(IGameObject owner, Vector2 position = default) : base(owner, position)
        {
            entity = owner as PhysicalEntity;
        }

        private Vector2 velocity;

        public override Vector2 Velocity
        {
            get => velocity;

            set => velocity = value;
        }

        public override float VelocityX
        {
            get => velocity.X;

            set => velocity.X = value;
        }

        public override float VelocityY
        {
            get => velocity.Y;

            set => velocity.Y = value;
        }

        internal Vector2 InternalVelocity
        {
            get
            {

                    return velocity;
            }

            set => velocity = value;
        }

        internal float InternalVelocityX
        {
            get
            {
                    return velocity.X;
            }

            set => velocity.X = value;
        }

        internal float InternalVelocityY
        {
            get
            {
                    return velocity.Y;
            }

            set => velocity.Y = value;
        }

        /*internal Vector2 InternalVelocity
        {
            get
            {
                if (entity.MountedOn == null)
                {
                    return velocity;
                }
                return entity.MountedOn.Transform.Velocity + velocity;
            }

            set => velocity = value;
        }

        internal float InternalVelocityX
        {
            get
            {
                if (entity.MountedOn == null)
                {
                    return velocity.X;
                }
                return entity.MountedOn.Transform.Velocity.X + velocity.X;
            }

            set => velocity.X = value;
        }

        internal float InternalVelocityY
        {
            get
            {
                if (entity.MountedOn == null)
                {
                    return velocity.Y;
                }
                return entity.MountedOn.Transform.Velocity.Y + velocity.Y;
            }

            set => velocity.Y = value;
        }*/
    }
}
