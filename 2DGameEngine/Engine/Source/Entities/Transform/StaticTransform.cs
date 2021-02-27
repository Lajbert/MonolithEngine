using GameEngine2D.Engine.Source.Entities.Abstract;
using GameEngine2D.Engine.Source.Entities.Transform;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.Engine.Source.Entities
{
    public class StaticTransform : AbstractTransform
    {
        public StaticTransform(IGameObject owner, Vector2 position = default(Vector2)) : base(owner, position)
        {

        }

        public override Vector2 Velocity
        {
            get => throw new NotImplementedException(); set => throw new NotImplementedException();
        }
    }
}
