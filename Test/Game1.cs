using GameEngine2D.Engine.Source.Util;
using GameEngine2D.Global;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Test
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        Texture2D rect;
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            Config.RES_W = 3840;
            Config.RES_W = 2160;
            Config.FULLSCREEN = true;

            //Config.GRID = 64;

            //this.IsFixedTimeStep = true;//false;
            //this.TargetElapsedTime = TimeSpan.FromSeconds(1d / 60); //60);

            // uncapped framerate
            _graphics.SynchronizeWithVerticalRetrace = false;
            this.IsFixedTimeStep = false;

            _graphics.PreferredBackBufferWidth = Config.RES_W;
            _graphics.PreferredBackBufferHeight = Config.RES_H;
            _graphics.IsFullScreen = Config.FULLSCREEN;
            _graphics.ApplyChanges();

            rect = new Texture2D(GraphicsDevice, Config.GRID, Config.GRID);
            Color[] data = new Color[Config.GRID * Config.GRID];
            for (int j = 0; j < data.Length; ++j) data[j] = Color.Black;
            rect.SetData(data);

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            for (int i = 0; i < 1000; i++)
            {
                
                _spriteBatch.Draw(rect, new Vector2(i * 100, 500), Color.White);
            }
            _spriteBatch.End();

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
