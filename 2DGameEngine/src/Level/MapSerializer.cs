using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace _2DGameEngine.src.Level
{
    interface MapSerializer
    {
        public LDTKMap Deserialize(String filePath);
    }
}
