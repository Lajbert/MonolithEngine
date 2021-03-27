using MonolithEngine.Engine.Source.Components;
using MonolithEngine.Engine.Source.Physics.Interface;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonolithEngine.Engine.Source.Physics.Collision
{
    public interface ICollisionComponent : IComponent
    {
        public bool CollidesWith(IColliderEntity otherCollider);

        public ColliderType GetType();

        public Vector2 Position { get; }
    }
}
