using System;
using System.Collections.Generic;
using System.Text;

namespace MonolithEngine.Engine.Source.Components
{
    public interface IComponent
    {
        public bool UniquePerEntity { get; set; }

        public Type GetComponentType()
        {
            return GetType();
        }
    }
}
