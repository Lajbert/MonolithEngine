using GameEngine2D.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForestPlatformerExample.Source.Entities.Interfaces
{
    interface IMovableItem
    {
        public void Lift(Entity entity, Vector2 newPosition);

        public void PutDown(Entity entity, Vector2 newPosition);

        public void Throw(Entity entity);
    }
}
