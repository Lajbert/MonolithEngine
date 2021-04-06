using MonolithEngine.Engine.Source.Entities.Transform;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonolithEngine.Engine.Source.Entities.Abstract
{
    public interface IGameObject
    {
        public AbstractTransform Transform { get; set; }

        public IGameObject Parent { get; }

        public ICollection<string> GetTags();

        public bool HasTag(string tag);

        public void AddChild(IGameObject gameObject);

        public void RemoveChild(IGameObject gameObject);

        public void Destroy();
    }
}
