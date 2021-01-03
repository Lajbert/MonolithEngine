using GameEngine2D.Engine.src.Util;
using GameEngine2D.Global;
using GameEngine2D.src;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.GameExamples.TopDown.src.Hero
{
    class CubeGuy : ControllableEntity
    {
        public CubeGuy(Vector2 position, SpriteFont font = null) : base(Scene.Instance.GetEntityLayer(), null, position, font)
        {
            //SetSprite(SpriteUtil.CreateRectangle(graphicsDeviceManager, Config.GRID, Color.White));
        }
    }
}
