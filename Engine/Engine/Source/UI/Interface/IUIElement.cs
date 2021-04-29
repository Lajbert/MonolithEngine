using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonolithEngine
{
    public interface IUIElement
    {

        public void Draw(SpriteBatch spriteBatch);

        public void Update(Point mousePosition = default);

        public IUIElement GetParent();

        public Vector2 GetPosition();
    }
}
