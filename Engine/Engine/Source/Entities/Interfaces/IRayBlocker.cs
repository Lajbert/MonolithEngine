using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace MonolithEngine
{
    public interface IRayBlocker
    {
        public bool BlocksRay { get; set; }
        public List<(Vector2 start, Vector2 end)> GetRayBlockerLines();
    }
}
