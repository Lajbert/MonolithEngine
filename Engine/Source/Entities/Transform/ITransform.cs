using Microsoft.Xna.Framework;

namespace MonolithEngine
{
    public interface ITransform
    {
        public Vector2 Position { get; set; }

        public float X { get; set; }
        
        public float Y { get; set; }

        public Vector2 Velocity { get; set; }

        public float Rotation { get; set; }

        public void OverridePositionOffset(Vector2 newPositionOffset);
    }
}
