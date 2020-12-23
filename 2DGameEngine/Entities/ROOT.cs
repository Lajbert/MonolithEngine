using System;
using System.Collections.Generic;
using System.Text;
using _2DGameEngine.Entities;
using _2DGameEngine.Entities.Interfaces;

namespace _2DGameEngine.Entities
{
    class ROOT : HasChildren
    {

        private static HashSet<HasParent> children = new HashSet<HasParent>();

        private static readonly ROOT instance = new ROOT();

        private ROOT() { }
        static ROOT() { }

        public void AddChild(HasParent gameObject)
        {
            children.Add(gameObject);
        }

        public HashSet<HasParent> GetAllChildren()
        {
            return children;
        }

        HashSet<HasParent> HasChildren.GetAllChildren()
        {
            return children;
        }

        public void RemoveChild(HasParent gameObject)
        {
            children.Remove(gameObject);
        }

        public static ROOT Instance
        {
            get
            {
                return instance;
            }
        }
    }
}
