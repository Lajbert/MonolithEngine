using GameEngine2D.Engine.Source.Graphics;
using GameEngine2D.Entities.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.Source.Entities.Animation.Interface
{
    public interface IAnimation
    {
        public void Update(GameTime gameTime);

        public void Play(SpriteBatch spriteBatch);
    }
}
