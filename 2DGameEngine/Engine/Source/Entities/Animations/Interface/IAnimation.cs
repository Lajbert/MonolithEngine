using MonolithEngine.Engine.Source.Graphics;
using MonolithEngine.Entities.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonolithEngine.Source.Entities.Animation.Interface
{
    public interface IAnimation
    {
        public void Update();

        public void Play(SpriteBatch spriteBatch);
    }
}
