using GameEngine2D.Engine.src.Entities;
using GameEngine2D.Engine.src.Entities.Controller;
using GameEngine2D.Engine.src.Util;
using GameEngine2D.Global;
using GameEngine2D.src;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.GameExamples.TopDown.src.Hero
{
    class CubeGuy : ControllableEntity
    {
        public CubeGuy(Vector2 position, SpriteFont font = null) : base(Scene.Instance.EntityLayer, null, position, font)
        {
            //SetSprite(SpriteUtil.CreateRectangle(graphicsDeviceManager, Config.GRID, Color.White));
            SetupController();
        }

        private void SetupController()
        {
            UserInput = new UserInputController();

            UserInput.RegisterControllerState(Keys.Right, () => {
                Direction.X += Config.CHARACTER_SPEED * elapsedTime;
                CurrentFaceDirection = Engine.src.Entities.FaceDirection.RIGHT;
            });

            UserInput.RegisterControllerState(Keys.Left, () => {
                Direction.X -= Config.CHARACTER_SPEED * elapsedTime;
                CurrentFaceDirection = Engine.src.Entities.FaceDirection.LEFT;
            });

            UserInput.RegisterControllerState(Keys.Down, () => {
                Direction.Y += Config.CHARACTER_SPEED * elapsedTime;
                CurrentFaceDirection = Engine.src.Entities.FaceDirection.DOWN;
            });

            UserInput.RegisterControllerState(Keys.Up, () => {
                Direction.Y -= Config.CHARACTER_SPEED * elapsedTime;
                CurrentFaceDirection = Engine.src.Entities.FaceDirection.UP;
            });
        }
    }
}
