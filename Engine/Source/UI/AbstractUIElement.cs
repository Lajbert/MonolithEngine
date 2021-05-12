using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonolithEngine
{
    /// <summary>
    /// Base class representing UI elements.
    /// UI elements move with the camera and are unaffected by the zoom level.
    /// </summary>
    public abstract class AbstractUIElement : IUIElement
    {
        public IUIElement Parent;
        public Vector2 Position;

        public AbstractUIElement(Vector2 position)
        {
            Position = position;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            
        }

        public IUIElement GetParent()
        {
            return Parent;
        }

        public Vector2 GetPosition()
        {
            if (Parent != null)
            {
                return (Parent.GetPosition() + Position);// * Config.SCALE;
            }
            return Position;// * Config.SCALE;
        }

        public virtual void Update(Point mousePosition = default)
        {
            
        }
    }
}
