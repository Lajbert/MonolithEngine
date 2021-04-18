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
    public class LDTKJsonMapParser : MapSerializer
    {
        public LDTKMap Load(string filePath)
        {
            return new LDTKMap(LDTKJson.FromJson(File.ReadAllText(filePath)));
        }
    }
}
