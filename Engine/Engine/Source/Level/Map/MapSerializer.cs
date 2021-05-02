namespace MonolithEngine
{
    /// <summary>
    /// Interface LDtk software map serializer.
    /// </summary>
    public interface MapSerializer
    {
        public LDTKMap Load(string filePath);
    }
}
