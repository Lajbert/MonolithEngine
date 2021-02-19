using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.Engine.Source.Components
{
    public interface IComponent
    {
        public bool IsCollection { get; set; }

        public Type GetComponentType()
        {
            return GetType();
        }
    }
}
