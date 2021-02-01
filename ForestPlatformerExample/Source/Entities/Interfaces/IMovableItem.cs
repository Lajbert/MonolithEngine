using GameEngine2D.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForestPlatformerExample.Source.Entities.Interfaces
{
    interface IMovableItem
    {
        public void Lift(Entity entity);

        public void PutDown(Entity entity);

        public void Throw(Entity entity);
    }
}
