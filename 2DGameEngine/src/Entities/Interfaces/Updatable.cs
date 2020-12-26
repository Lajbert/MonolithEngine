using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace _2DGameEngine.Entities.Interfaces
{
    interface Updatable
    {
        public void PreUpdate(GameTime gameTime);
        public void Update(GameTime gameTime);
        public void PostUpdate(GameTime gameTime);
    }
}
