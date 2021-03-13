using GameEngine2D.Engine.Source.Components;
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
            get => PositionOffset + owner.Transform.Position;
        }
        public bool UniquePerEntity { get; set; }

        protected IColliderEntity owner;

        protected AbstractCollisionComponent(ColliderType type, IColliderEntity owner, Vector2 positionOffset = default)
        {
            this.owner = owner;
            PositionOffset = positionOffset;
            this.type = type;
            UniquePerEntity = true;
        }

        public abstract bool CollidesWith(IColliderEntity otherCollider);

        ColliderType ICollisionComponent.GetType()
        {
            return type;
        }

        Type IComponent.GetComponentType()
        {
            return typeof(ICollisionComponent);
        }

#if DEBUG
        protected abstract void CreateDebugVisual();
#endif
    }
}
