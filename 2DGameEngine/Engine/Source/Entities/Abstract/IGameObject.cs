using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.Engine.Source.Entities.Abstract
{
    public interface IGameObject
    {
        public Vector2 GetPosition();

        public ICollection<string> GetTags();
    }
}
