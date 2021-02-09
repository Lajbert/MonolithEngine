using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.Engine.Source.Entities.Interfaces
{
    public interface IGridCollider
    {
        Vector2 GetInCellLocation();
        Vector2 GetGridCoord();
        Vector2 GetPosition();

        public bool HasTag(string tag);

        public float GetCollisionOffset(Direction direction);

        public bool BlocksMovementFrom(Direction direction);
    }
}
