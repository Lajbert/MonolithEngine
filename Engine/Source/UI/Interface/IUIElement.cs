﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

namespace MonolithEngine
{
    public interface IUIElement
    {

        public void Draw(SpriteBatch spriteBatch);

        public void Update(Point mousePosition = default);

        public void Update(TouchCollection touchLocations);

        public IUIElement GetParent();

        public Vector2 GetPosition();
    }
}
