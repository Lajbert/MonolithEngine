using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.Entities.Interfaces
{
    public interface ICollider
    {
        public bool HasCollision { get; set; }

        public Vector2 Position { get; set; }
    }
}
