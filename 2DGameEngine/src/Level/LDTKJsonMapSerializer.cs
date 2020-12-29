using _2DGameEngine.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace _2DGameEngine.src.Level
{
    class LDTKJsonMapSerializer : MapSerializer
    {
        public LDTKMap Deserialize(String filePath)
        {
            return JsonSerializer.Deserialize<LDTKMap>(File.ReadAllText(filePath));
        }
    }
}
