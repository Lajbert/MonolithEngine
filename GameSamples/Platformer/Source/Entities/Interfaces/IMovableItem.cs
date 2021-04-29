using Microsoft.Xna.Framework;
using MonolithEngine;

namespace ForestPlatformerExample
{
    interface IMovableItem
    {
        public void Lift(Entity entity, Vector2 newPosition);

        public void PutDown(Entity entity, Vector2 newPosition);

        public void Throw(Entity entity, Vector2 force);
    }
}
