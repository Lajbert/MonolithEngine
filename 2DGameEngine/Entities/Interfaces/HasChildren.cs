using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace _2DGameEngine.Entities.Interfaces
{
    interface HasChildren
    {
        public HashSet<HasParent> GetAllChildren();

        public void AddChild(HasParent gameObject);

        public void RemoveChild(HasParent gameObject);

        public Vector2 GetPosition();
    }
}
