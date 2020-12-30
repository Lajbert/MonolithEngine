using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.Entities.Interfaces
{
    interface IHasParent
    {
        public Entity GetParent();

        public void AddParent(Entity newParent);
    }
}
