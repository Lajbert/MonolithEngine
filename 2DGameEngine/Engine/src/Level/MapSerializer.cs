using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.src.Level
{
    interface MapSerializer
    {
        public LDTKMap Deserialize(String filePath);
    }
}
