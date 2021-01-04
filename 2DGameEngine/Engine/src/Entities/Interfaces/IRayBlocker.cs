using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.Engine.src.Entities.Interfaces
{
    public interface IRayBlocker
    {
        public bool BlocksRay { get; set; }
        public List<(Vector2 start, Vector2 end)> GetRayBlockerLines();
    }
}
