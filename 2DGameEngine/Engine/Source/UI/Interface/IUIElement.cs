using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonolithEngine.Engine.Source.UI
{
    public interface IUIElement
    {

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime);

        public void Update(Point mousePosition = default);
    }
}
