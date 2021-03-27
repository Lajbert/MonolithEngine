using MonolithEngine.Engine.Source.Entities.Abstract;
using MonolithEngine.Engine.Source.Entities.Transform;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonolithEngine.Engine.Source.Entities
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
