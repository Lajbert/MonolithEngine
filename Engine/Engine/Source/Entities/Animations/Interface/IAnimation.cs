using Microsoft.Xna.Framework.Graphics;

namespace MonolithEngine
{
    public interface IAnimation
    {
        public void Update();

        public void Play(SpriteBatch spriteBatch);
    }
}
