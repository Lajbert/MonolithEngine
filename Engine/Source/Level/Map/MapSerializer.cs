using System.IO;

namespace MonolithEngine
{
    /// <summary>
    /// Interface LDtk software map serializer.
    /// </summary>
    public interface MapSerializer
    {
        public LDTKMap Load(string filePath);

        public LDTKMap Load(Stream stream);
    }
}
