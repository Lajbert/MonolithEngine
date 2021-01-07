using GameEngine2D.Engine.Source.Level;
using GameEngine2D.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace GameEngine2D.Source.Level
{
    public class LDTKJsonMapSerializer : MapSerializer
    {
        public LDTKMap Deserialize(string filePath)
        {
            return new LDTKMap(LDTKJson.FromJson(File.ReadAllText(filePath)));
            //return JsonSerializer.Deserialize<LDTKMap>(File.ReadAllText(filePath));
        }
    }
}
