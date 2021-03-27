using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonolithEngine.Engine.Source.Interfaces
{
    public interface IDrawableComponent
    {
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime);
    }
}
