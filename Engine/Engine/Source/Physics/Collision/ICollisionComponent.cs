using Microsoft.Xna.Framework;

namespace MonolithEngine
{
    public interface ICollisionComponent : IComponent
    {
        public bool CollidesWith(IColliderEntity otherCollider);

        public ColliderType GetType();

        public Vector2 Position { get; }
    }
}
