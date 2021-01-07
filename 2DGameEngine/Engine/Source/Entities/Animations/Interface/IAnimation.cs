using GameEngine2D.Entities.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.Source.Entities.Animation.Interface
{
    public interface IAnimation
    {
        public void Update(GameTime gameTime);

        public void Play();
    }
}
