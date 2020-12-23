using System;
using System.Collections.Generic;
using System.Text;
using _2DGameEngine.Entities;
using _2DGameEngine.Entities.Interfaces;

namespace _2DGameEngine.Entities
{
    class ROOT : HasChildren
    {

        private static List<HasParent> children = new List<HasParent>();

        private static readonly ROOT instance = new ROOT();

        private ROOT() { }
        static ROOT() { }

        public void AddChild(HasParent gameObject)
        {
            children.Add(gameObject);
        }

        public List<HasParent> GetAllChildren()
        {
            return children;
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
