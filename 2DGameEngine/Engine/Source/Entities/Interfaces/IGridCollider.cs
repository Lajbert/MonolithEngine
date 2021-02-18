using GameEngine2D.Engine.Source.Entities.Transform;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.Engine.Source.Entities.Interfaces
{
    public interface IGridCollider
    {

        public AbstractTransform Transform { get; set; }

        public bool HasTag(string tag);

        public float GetCollisionOffset(Direction direction);

        public bool BlocksMovementFrom(Direction direction);
    }
}
