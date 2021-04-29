using System.IO;

namespace MonolithEngine
{
    public class LDTKJsonMapParser : MapSerializer
    {
        public LDTKMap Load(string filePath)
        {
            return new LDTKMap(LDTKJson.FromJson(File.ReadAllText(filePath)));
        }
    }
}
