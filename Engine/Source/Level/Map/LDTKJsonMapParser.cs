using System.IO;

namespace MonolithEngine
{
    /// <summary>
    /// A parser class for the output of LDtk level editor software.
    /// </summary>
    public class LDTKJsonMapParser : MapSerializer
    {
        public LDTKMap Load(string filePath)
        {
            return new LDTKMap(LDTKJson.FromJson(File.ReadAllText(filePath)));
        }

        LDTKMap MapSerializer.Load(Stream stream)
        {
            return new LDTKMap(LDTKJson.FromJson(new StreamReader(stream).ReadToEnd()));
        }
    }
}
