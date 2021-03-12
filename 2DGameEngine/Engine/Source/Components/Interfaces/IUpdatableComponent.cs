using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.Engine.Source.Interfaces
{
    public interface IUpdatableComponent
    {
        public void Update(GameTime gameTime);
    }
}
