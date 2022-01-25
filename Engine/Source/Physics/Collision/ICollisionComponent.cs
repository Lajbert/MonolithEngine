using Microsoft.Xna.Framework;

namespace MonolithEngine
{
    /// <summary>
    /// Interface representing a collision component.
    /// </summary>
    public interface ICollisionComponent : IComponent
    {
        public bool CollidesWith(IColliderEntity otherCollider);

        public ColliderType GetType();

        public Vector2 Position { get; }
    }
}
