using _2DGameEngine.Entities.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace _2DGameEngine.src.Entities.Animation.Interface
{
    interface IAnimation
    {
        public void Update(GameTime gameTime);

        public void Draw(Vector2 position);
    }
}
