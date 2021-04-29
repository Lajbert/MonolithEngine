using Microsoft.Xna.Framework;

namespace MonolithEngine
{
    class DynamicTransform : StaticTransform
    {
        public DynamicTransform(IGameObject owner, Vector2 position = default) : base(owner, position)
        {

        }

        public override Vector2 Velocity { get; set; }
    }
}
