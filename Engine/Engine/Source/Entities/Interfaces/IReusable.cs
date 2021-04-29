using Microsoft.Xna.Framework;

namespace MonolithEngine
{
    public interface IReusable
    {
        public void Reset(Vector2 position = new Vector2());
    }
}
