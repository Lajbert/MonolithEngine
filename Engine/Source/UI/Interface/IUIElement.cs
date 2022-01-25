using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System.Collections.Generic;

namespace MonolithEngine
{
    public interface IUIElement
    {

        public void Draw(SpriteBatch spriteBatch);

        public void Update(Point mousePosition = default);

        public void Update(TouchCollection touchLocations);

        public void FixedUpdate();

        public IUIElement GetParent();

        public void SetParent(IUIElement parent);

        public HashSet<IUIElement> GetChildren();

        public void AddChild(IUIElement child);

        public Vector2 GetPosition();
    }
}
