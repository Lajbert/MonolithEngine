using System;

namespace MonolithEngine
{

    /// <summary>
    /// Represents a component that can be assigned to an entity
    /// </summary>
    public interface IComponent
    {
        // marks if multiple instances of the same component can be assigned to an entity
        public bool UniquePerEntity { get; set; }

        public Type GetComponentType();
    }
}
