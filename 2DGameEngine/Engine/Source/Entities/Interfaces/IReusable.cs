using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameEngine2D.Entities.Interfaces
{
    public interface IReusable
    {
        public void Reset(Vector2 position = new Vector2());
    }
}
