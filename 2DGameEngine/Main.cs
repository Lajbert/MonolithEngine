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
        private Random random;

        public Main()
        {
            // >>>>>>> set framerate >>>>>>>>>>
            //this.IsFixedTimeStep = true;//false;
            //this.TargetElapsedTime = TimeSpan.FromSeconds(1d / Constants.FPS); //60);
            
            graphics = new GraphicsDeviceManager(this);
            //graphics.IsFullScreen = true;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            random = new Random();

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
            hero = new ControllableEntity(null, graphics.GraphicsDevice, CreateRectangle(Constants.GRID, Color.Blue), new Vector2(5, 1) * Constants.GRID, font);
            Entity child = new Entity(hero, graphics.GraphicsDevice, CreateRectangle(Constants.GRID, Color.Black), new Vector2(1 , 0) * Constants.GRID, font);
            Entity child2 = new Entity(child, graphics.GraphicsDevice, CreateRectangle(Constants.GRID, Color.Red), new Vector2(1, 0) * Constants.GRID, font);
            Entity child3 = new Entity(child2, graphics.GraphicsDevice, CreateRectangle(Constants.GRID, Color.Blue), new Vector2(1, 0) * Constants.GRID, font);
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
                
                Color c = Color.FromNonPremultiplied(random.Next(256), random.Next(256), random.Next(256), 256);
                Entity e = new Entity(null, graphics.GraphicsDevice, CreateRectangle(Constants.GRID, c), new Vector2(i, 17 * Constants.GRID), font);
            }

            for (int i = 16 * Constants.GRID; i < 27 * Constants.GRID; i += Constants.GRID)
            {
                Color c = Color.FromNonPremultiplied(random.Next(256), random.Next(256), random.Next(256), 256);
                Entity e = new Entity(null, graphics.GraphicsDevice, CreateRectangle(Constants.GRID, c), new Vector2(i, 15 * Constants.GRID), font);
            }

            for (int i = 2 * Constants.GRID; i < 25 * Constants.GRID; i+= Constants.GRID)
            {
                Color c = Color.FromNonPremultiplied(random.Next(256), random.Next(256), random.Next(256), 256);
                Entity e = new Entity(null, graphics.GraphicsDevice, CreateRectangle(Constants.GRID, c), new Vector2(i, 20 * Constants.GRID), font);
            }

            for (int i = 9 * Constants.GRID; i < 10 * Constants.GRID; i += Constants.GRID)
            {
                Color c = Color.FromNonPremultiplied(random.Next(256), random.Next(256), random.Next(256), 256);
                Entity e = new Entity(null, graphics.GraphicsDevice, CreateRectangle(Constants.GRID, c), new Vector2(i, 19 * Constants.GRID), font);
            }

            for (int i = 25 * Constants.GRID; i < 50 * Constants.GRID; i += Constants.GRID)
            {
                Color c = Color.FromNonPremultiplied(random.Next(256), random.Next(256), random.Next(256), 256);
                Entity e = new Entity(null, graphics.GraphicsDevice, CreateRectangle(Constants.GRID, c), new Vector2(i, 19 * Constants.GRID), font);
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

        float deltaTime = 0;
        protected override void Draw(GameTime gameTime)
        {
            deltaTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            GraphicsDevice.Clear(Color.Lerp(Color.Green, Color.Orange, (float)Math.Sin(deltaTime)));

            // TODO: Add your drawing code here
            RootContainer.Instance.DrawAll(gameTime);
            base.Draw(gameTime);
        }
    }
}
