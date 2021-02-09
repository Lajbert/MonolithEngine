using GameEngine2D.Engine.Source.Physics.Interface;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.Engine.Source.Physics.Collision
{
    public interface ICollisionComponent
    {
        public bool Overlaps(IColliderEntity otherCollider);

        public Vector2 Position { get; }
    }
}
