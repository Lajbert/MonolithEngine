using GameEngine2D.Engine.Source.Physics.Collision;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.Engine.Source.Physics.Interface
{
    public interface ICircleCollider
    {
        public CircleCollider CircleCollider { get; set; }

        public Vector2 GetPosition();

    }
}
