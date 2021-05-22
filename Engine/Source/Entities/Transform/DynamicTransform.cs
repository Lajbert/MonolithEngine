using Microsoft.Xna.Framework;
using System;

namespace MonolithEngine
{
    /// <summary>
    /// A Transform that has Velocity
    /// </summary>
    class DynamicTransform : StaticTransform
    {

        public DynamicTransform(IGameObject owner, Vector2 position = default) : base(owner, position)
        {
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
    }
}
