using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace MonolithEngine
{
    /// <summary>
    /// Represents that the entity blocks rays while raycasting.
    /// </summary>
    public interface IRayBlocker
    {
        public bool BlocksRay { get; set; }
        public List<(Vector2 start, Vector2 end)> GetRayBlockerLines();
    }
}
