using Microsoft.Xna.Framework;
using System;

namespace MonolithEngine
{
    /// <summary>
    /// A transform that doesn't have velocity
    /// </summary>
    public class StaticTransform : AbstractTransform
    {
        public StaticTransform(IGameObject owner, Vector2 position = default) : base(owner, position)
        {

        }

        public override Vector2 Velocity
        {
            get => throw new NotImplementedException(); set => throw new NotImplementedException();
        }
        public override float VelocityX { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override float VelocityY { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
