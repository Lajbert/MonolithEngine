using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using MonolithEngine.Entities;
using MonolithEngine.Entities.Interfaces;
using System.Collections.Generic;
using MonolithEngine.Global;
using MonolithEngine.Source.Level;
using MonolithEngine.Source;
using MonolithEngine.Source.Entities.Animation;
using MonolithEngine.Util;

namespace MonolithEngine
{
    public class Main : Game
    {
        private GraphicsDeviceManager graphics;

        public Main()
        {
            // >>>>>>> set framerate >>>>>>>>>>
            this.IsFixedTimeStep = true;//false;
            //this.TargetElapsedTime = TimeSpan.FromSeconds(1d / Config.FPS); //60);

            graphics = new GraphicsDeviceManager(this)
            {
                //IsFullScreen = Config.FULLSCREEN
            };
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
            //graphics.PreferredBackBufferWidth = Config.RES_W;
            //graphics.PreferredBackBufferHeight = Config.RES_H;
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
