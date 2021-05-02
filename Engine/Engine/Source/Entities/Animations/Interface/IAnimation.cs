using Microsoft.Xna.Framework.Graphics;

namespace MonolithEngine
{
    /// <summary>
    /// Interface to represent an animation
    /// </summary>
    public interface IAnimation
    {
        public void Update();

        public void Play(SpriteBatch spriteBatch);
    }
}
