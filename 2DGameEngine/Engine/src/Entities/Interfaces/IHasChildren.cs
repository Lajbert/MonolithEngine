using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.Entities.Interfaces
{
    interface IHasChildren
    {
        public HashSet<Entity> GetAllChildren();

        public void AddChild(Entity gameObject);

        public void RemoveChild(Entity gameObject);
    }
}
