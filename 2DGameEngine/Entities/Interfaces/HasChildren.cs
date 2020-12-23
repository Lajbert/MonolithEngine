using System;
using System.Collections.Generic;
using System.Text;

namespace _2DGameEngine.Entities.Interfaces
{
    interface HasChildren
    {
        public List<HasParent> GetAllChildren();

        public void AddChild(HasParent gameObject);
    }
}
