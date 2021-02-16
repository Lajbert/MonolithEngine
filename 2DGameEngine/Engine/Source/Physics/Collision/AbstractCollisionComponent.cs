using GameEngine2D.Engine.Source.Physics.Interface;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.Engine.Source.Physics.Collision
{
    public abstract class AbstractCollisionComponent : ICollisionComponent
    {
        protected Vector2 PositionOffset;

        protected ColliderType type;

#if DEBUG
        private bool showDebug = false;
        public bool DEBUG_DISPLAY_COLLISION
        {
            get => showDebug;
            set
            {
                if (value != showDebug)
                {
                    CreateDebugVisual();
                }
                showDebug = value;
            }
        }
#endif

        public Vector2 Position
        {
            get => PositionOffset + owner.GetPosition();
        }

        protected IColliderEntity owner;

        protected AbstractCollisionComponent(ColliderType type, IColliderEntity owner, Vector2 positionOffset = default(Vector2))
        {
            this.owner = owner;
            PositionOffset = positionOffset;
            this.type = type;
        }

        public abstract bool Overlaps(IColliderEntity otherCollider);

        ColliderType ICollisionComponent.GetType()
        {
            return type;
        }

#if DEBUG
        protected abstract void CreateDebugVisual();
#endif
    }
}
