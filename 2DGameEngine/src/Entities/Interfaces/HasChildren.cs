using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace _2DGameEngine.Entities.Interfaces
{
    interface HasChildren
    {
        public HashSet<Entity> GetAllChildren();

        public void AddChild(Entity gameObject);

        public void RemoveChild(Entity gameObject);
    }
}
