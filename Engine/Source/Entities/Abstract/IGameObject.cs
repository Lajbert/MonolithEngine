using System.Collections.Generic;

namespace MonolithEngine
{

    /// <summary>
    /// Interface for game objects
    /// </summary>
    public interface IGameObject
    {
        public AbstractTransform Transform { get; set; }

        public IGameObject Parent { get; }

        public ICollection<string> GetTags();

        public bool HasTag(string tag);

        public void AddChild(IGameObject gameObject);

        public void RemoveChild(IGameObject gameObject);

        public void Destroy();

        public bool IsAlive();
    }
}
