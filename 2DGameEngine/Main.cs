﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using GameEngine2D.Entities;
using GameEngine2D.Entities.Interfaces;
using System.Collections.Generic;
using GameEngine2D.Global;
using GameEngine2D.src.Camera;
using GameEngine2D.src.Level;
using GameEngine2D.src;
using GameEngine2D.src.Entities.Animation;
using GameEngine2D.Util;

namespace GameEngine2D
{
    public class Main : Game
    {
        private GraphicsDeviceManager graphics;

        public Main()
        {
            // >>>>>>> set framerate >>>>>>>>>>
            this.IsFixedTimeStep = true;//false;
            this.TargetElapsedTime = TimeSpan.FromSeconds(1d / Constants.FPS); //60);

            graphics = new GraphicsDeviceManager(this);
            graphics.IsFullScreen = Constants.FULLSCREEN;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            base.Initialize();
        }

        protected override void LoadContent()
        {
            graphics.PreferredBackBufferWidth = Constants.RES_W;
            graphics.PreferredBackBufferHeight = Constants.RES_H;
            graphics.ApplyChanges();
        }
            
        

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}
