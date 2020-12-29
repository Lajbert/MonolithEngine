using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace _2DGameEngine.src.Level
{
    class LDTKMap
    {

        public HashSet<Vector2> GetCollisions()
        {
            HashSet<Vector2> collisions = new HashSet<Vector2>();
            foreach (Level level in levels)
            {
                foreach (LayerInstance layer in level.layerInstances)
                {
                    foreach (LayerInstance.IntGrid grid in layer.intGrid)
                    {
                        //var coordId = gridBasedX + gridBasedY * gridBasedWidth;
                        //var gridBasedY = Math.floor( coordId / gridBasedWidth );
                        //var gridBasedX = coordId - gridBasedY * gridBasedWidth
                        //cx / cy for a grid based X / Y
                        //cWid / cHei for a grid based width / height

                        int y = (int)Math.Floor((decimal)grid.coordId / 35);
                        int x = grid.coordId - y * 35;
                        
                        collisions.Add(new Vector2(x, y));
                    }
                }
            }

            return collisions;
        }

        public string jsonVersion { get; set; }

        public IList<Level> levels { get; set; }

        public class Level
        {
            public string identifier { get; set; }
            public IList<LayerInstance> layerInstances { get; set; }
        }

        public class LayerInstance
        {
            public List<IntGrid> intGrid { get; set; }

            public string __identifier { get; set; }

            public class IntGrid
            {
                public int coordId { get; set; }
                public int v { get; set; }
            }
        }
    }
}
