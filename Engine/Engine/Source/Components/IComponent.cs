using System;

namespace MonolithEngine
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
