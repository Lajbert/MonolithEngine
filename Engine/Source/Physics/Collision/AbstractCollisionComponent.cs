using Microsoft.Xna.Framework;
using System;

namespace MonolithEngine
{
    /// <summary>
    /// Abstract base class for dynamic collision components that can be
    /// assigned to entities. It can be used to detect collisions between
    /// moving objects.
    /// The collision detection is accurate: checks if the collier shapes
    /// themselves are intersecting or not.
    /// </summary>
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

        /// <summary>
        /// Returns whether the 2 collisions components are intersecting or not.
        /// </summary>
        /// <param name="otherCollider"></param>
        /// <returns></returns>
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
