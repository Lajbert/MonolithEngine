using GameEngine2D.Engine.Source.Entities;
using GameEngine2D.Engine.Source.Entities.Controller;
using GameEngine2D.Engine.Source.Util;
using GameEngine2D.Entities;
using GameEngine2D.Global;
using GameEngine2D.Source;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.GameExamples.TopDown.Source.Hero
{
    class CubeGuy : ControllableEntity
    {
        public CubeGuy(Vector2 position, SpriteFont font = null) : base(RootContainer.Instance.EntityLayer, null, position, null, false, font)
        {
            SetSprite(SpriteUtil.CreateRectangle(Config.GRID, Color.Black));
            SetupController();
        }

        private void SetupController()
        {
            UserInput = new UserInputController();

            UserInput.RegisterControllerState(Keys.Right, () => {
                Direction.X += Config.CHARACTER_SPEED * elapsedTime;
                CurrentFaceDirection = Engine.Source.Entities.Direction.RIGHT;
            });

            UserInput.RegisterControllerState(Keys.Left, () => {
                Direction.X -= Config.CHARACTER_SPEED * elapsedTime;
                CurrentFaceDirection = Engine.Source.Entities.Direction.LEFT;
            });

            UserInput.RegisterControllerState(Keys.Down, () => {
                Direction.Y += Config.CHARACTER_SPEED * elapsedTime;
                CurrentFaceDirection = Engine.Source.Entities.Direction.DOWN;
            });

            UserInput.RegisterControllerState(Keys.Up, () => {
                Direction.Y -= Config.CHARACTER_SPEED * elapsedTime;
                CurrentFaceDirection = Engine.Source.Entities.Direction.UP;
            });
        }
    }
}
