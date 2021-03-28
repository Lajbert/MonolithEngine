using MonolithEngine.Engine.Source.Level;
using MonolithEngine.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using MonolithEngine.Engine.Source.Scene;

namespace MonolithEngine.Source.Level
{
    public class LDTKJsonMapSerializer : MapSerializer
    {
        public LDTKMap Deserialize(AbstractScene scene, string filePath)
        {
            return new LDTKMap(scene, LDTKJson.FromJson(File.ReadAllText(filePath)));
            //return JsonSerializer.Deserialize<LDTKMap>(File.ReadAllText(filePath));
        }
    }
}
