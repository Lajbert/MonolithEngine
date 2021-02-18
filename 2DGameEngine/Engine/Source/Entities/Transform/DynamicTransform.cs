using GameEngine2D.Engine.Source.Entities.Abstract;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.Engine.Source.Entities.Transform
{
    class DynamicTransform : StaticTransform
    {
        public DynamicTransform(IGameObject owner, Vector2 position = default(Vector2)) : base(owner, position)
        {

        }

        public override Vector2 Velocity { get; set; }
    }
}
