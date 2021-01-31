using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.Source.Level
{
    public interface MapSerializer
    {
        public LDTKMap Deserialize(string filePath);
    }
}
