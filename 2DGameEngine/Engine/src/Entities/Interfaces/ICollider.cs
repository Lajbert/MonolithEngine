using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.Entities.Interfaces
{
    interface ICollider
    {
        public bool HasCollision();

        public void SetCollisions(bool detectCollision);

        public Vector2 GetPosition();
    }
}
