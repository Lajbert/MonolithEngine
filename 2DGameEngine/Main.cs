using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using _2DGameEngine.Entities;
using _2DGameEngine.Entities.Interfaces;
using System.Collections.Generic;

namespace _2DGameEngine
{
    public class Main : Game
    {
        private GraphicsDeviceManager graphics;
        ControllableEntity hero;

        public Main()
        {
            // >>>>>>> set framerate >>>>>>>>>>
            /*
            this.IsFixedTimeStep = true;//false;
            this.TargetElapsedTime = TimeSpan.FromSeconds(1d / 60d); //60);
            */
            graphics = new GraphicsDeviceManager(this);
            //graphics.IsFullScreen = true;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            // uncapped framerate
            /*graphics.SynchronizeWithVerticalRetrace = false;
            this.IsFixedTimeStep = false;*/
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            
            Vector2 startPosition = new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2);

            hero = new ControllableEntity(ROOT.Instance, graphics.GraphicsDevice, CreateRectangle(30, Color.Black), startPosition);
            EntityManager.AddObject(hero);
            // TODO: use this.Content to load your game content here
            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;
            graphics.ApplyChanges();
        }

        private Texture2D CreateRectangle(int size, Color color)
        {
            Texture2D rect = new Texture2D(graphics.GraphicsDevice, size, size);
            Color[] data = new Color[size * size];
            for (int i = 0; i < data.Length; ++i) data[i] = color;
            rect.SetData(data);
            return rect;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            EntityManager.UpdateAll(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            EntityManager.DrawAll(gameTime);
            base.Draw(gameTime);
        }
    }
}
