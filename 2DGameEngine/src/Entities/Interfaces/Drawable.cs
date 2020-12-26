﻿using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace _2DGameEngine.Entities.Interfaces
{
    interface Drawable
    {

        public void PreDraw(GameTime gameTime);
        public void Draw(GameTime gameTime);
        public void PostDraw(GameTime gameTime);
    }
}
