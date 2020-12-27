using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using _2DGameEngine.Entities;
using _2DGameEngine.Entities.Interfaces;
using System.Collections.Generic;
using _2DGameEngine.Global;
using _2DGameEngine.src.Camera;

namespace _2DGameEngine
{
    public class Main : Game
    {
        private GraphicsDeviceManager graphics;
        private ControllableEntity hero;
        private SpriteFont font;
        private Camera camera;

        public Main()
        {
            // >>>>>>> set framerate >>>>>>>>>>
            //this.IsFixedTimeStep = true;//false;
            //this.TargetElapsedTime = TimeSpan.FromSeconds(1d / Constants.FPS); //60);
            
            graphics = new GraphicsDeviceManager(this);
            //graphics.IsFullScreen = true;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            // uncapped framerate
            graphics.SynchronizeWithVerticalRetrace = false;
            this.IsFixedTimeStep = false;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            camera = new Camera();
            font = Content.Load<SpriteFont>("DefaultFont");
            Vector2 startPosition = new Vector2(9 * Constants.GRID, 9 * Constants.GRID);
            hero = new ControllableEntity(null, graphics.GraphicsDevice, CreateRectangle(Constants.GRID, Color.Blue), startPosition, font);
            Entity child = new Entity(hero, graphics.GraphicsDevice, CreateRectangle(Constants.GRID, Color.BlueViolet), new Vector2(8 , 8) * Constants.GRID, font);
            // TODO: use this.Content to load your game content here
            graphics.PreferredBackBufferWidth = Constants.RES_W;
            graphics.PreferredBackBufferHeight = Constants.RES_H;
            graphics.ApplyChanges();
            CreateLevel();
            camera.trackTarget(hero, true);
        }

        private void CreateLevel()
        {
            for (int i = 2 * Constants.GRID; i < 15 * Constants.GRID; i += Constants.GRID)
            {
                Entity e = new Entity(null, graphics.GraphicsDevice, CreateRectangle(Constants.GRID, Color.Black), new Vector2(i, 17 * Constants.GRID), font);
            }

            for (int i = 16 * Constants.GRID; i < 27 * Constants.GRID; i += Constants.GRID)
            {
                Entity e = new Entity(null, graphics.GraphicsDevice, CreateRectangle(Constants.GRID, Color.Black), new Vector2(i, 15 * Constants.GRID), font);
            }

            for (int i = 2 * Constants.GRID; i < 25 * Constants.GRID; i+= Constants.GRID)
            {
                Entity e = new Entity(null, graphics.GraphicsDevice, CreateRectangle(Constants.GRID, Color.Black), new Vector2(i, 20 * Constants.GRID), font);
            }

            for (int i = 9 * Constants.GRID; i < 10 * Constants.GRID; i += Constants.GRID)
            {
                Entity e = new Entity(null, graphics.GraphicsDevice, CreateRectangle(Constants.GRID, Color.Black), new Vector2(i, 19 * Constants.GRID), font);
            }

            for (int i = 25 * Constants.GRID; i < 50 * Constants.GRID; i += Constants.GRID)
            {
                Entity e = new Entity(null, graphics.GraphicsDevice, CreateRectangle(Constants.GRID, Color.Black), new Vector2(i, 19 * Constants.GRID), font);
            }
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
            RootContainer.Instance.UpdateAll(gameTime);
            camera.update(gameTime);
            camera.postUpdate(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            RootContainer.Instance.DrawAll(gameTime);
            base.Draw(gameTime);
        }
    }
}
