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

        protected ColliderType type;

        public Vector2 Position
        {
            get => positionOffset + owner.GetPosition();
        }

        protected IColliderEntity owner;

        protected AbstractCollisionComponent(ColliderType type, IColliderEntity owner, Vector2 positionOffset = default(Vector2))
        {
            this.owner = owner;
            this.positionOffset = positionOffset;
            this.type = type;
        }

        public abstract bool Overlaps(IColliderEntity otherCollider);

        ColliderType ICollisionComponent.GetType()
        {
            return type;
        }
    }
}
