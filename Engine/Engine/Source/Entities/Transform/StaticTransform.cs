using Microsoft.Xna.Framework;
using System;

namespace MonolithEngine
{
    public class StaticTransform : AbstractTransform
    {
        public StaticTransform(IGameObject owner, Vector2 position = default) : base(owner, position)
        {

        }

        public override Vector2 Velocity
        {
            get => throw new NotImplementedException(); set => throw new NotImplementedException();
        }
    }
}
