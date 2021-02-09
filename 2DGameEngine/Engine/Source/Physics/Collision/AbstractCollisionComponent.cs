using GameEngine2D.Engine.Source.Physics.Interface;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.Engine.Source.Physics.Collision
{
    public abstract class AbstractCollisionComponent : ICollisionComponent
    {
        private Vector2 positionOffset;
        public Vector2 Position
        {
            get => positionOffset + owner.GetPosition();
        }

        protected IColliderEntity owner;

        public AbstractCollisionComponent(IColliderEntity owner, Vector2? positionOffset)
        {
            this.owner = owner;
            this.positionOffset = positionOffset != null ? positionOffset.Value : Vector2.Zero;
        }

        public abstract bool Overlaps(IColliderEntity otherCollider);
    }
}
