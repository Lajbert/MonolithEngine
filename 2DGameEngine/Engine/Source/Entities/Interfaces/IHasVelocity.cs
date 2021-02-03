using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.Engine.Source.Entities.Interfaces
{
    public interface IHasVelocity
    {
        public Vector2 GetVelocity();

        public void AddForce(Vector2 force);
    }
}
