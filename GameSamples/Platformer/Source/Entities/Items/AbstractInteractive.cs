using MonolithEngine;
using Microsoft.Xna.Framework;

namespace ForestPlatformerExample
{
    class AbstractInteractive : AbstractDestroyable
    {
        public AbstractInteractive(AbstractScene scene, Vector2 position) : base(scene, position) {
            AddTag("Interactive");
        }
    }
}
