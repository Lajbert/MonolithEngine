using Microsoft.Xna.Framework;
using MonolithEngine.Engine.Source.Scene;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonolithEngine.Source.Level
{
    public interface MapSerializer
    {
        public LDTKMap Load(string filePath);
    }
}
