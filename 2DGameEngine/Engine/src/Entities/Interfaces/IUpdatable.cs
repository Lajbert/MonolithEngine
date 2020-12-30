using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameEngine2D.Entities.Interfaces
{
    interface IUpdatable
    {
        public void PreUpdate(GameTime gameTime);
        public void Update(GameTime gameTime);
        public void PostUpdate(GameTime gameTime);
    }
}
