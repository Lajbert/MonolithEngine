using GameEngine2D.Engine.src.Util;
using GameEngine2D.Entities;
using GameEngine2D.GameExamples.SideScroller.src.Hero;
using GameEngine2D.Global;
using GameEngine2D.src;
using GameEngine2D.src.Camera;
using GameEngine2D.src.Level;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SideScrollerExample.SideScroller.src.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SideScrollerExample
{
    public class SideScrollerGame : Game
    {
        private GraphicsDeviceManager graphics;
        private ContentManager contentManager;
        private SpriteFont font;
        private Camera camera;
        private Random random;
        private Color background1;
        private Color background2;
        private float sin;
        private MapSerializer mapSerializer;
        private float elapsedTime = 0;

        private FrameCounter frameCounter;

        private SpriteBatch spriteBatch;

        public SideScrollerGame()
        {
            // >>>>>>> set framerate >>>>>>>>>>
            this.IsFixedTimeStep = true;//false;
            this.TargetElapsedTime = TimeSpan.FromSeconds(1d / Config.FPS); //60);

            graphics = new GraphicsDeviceManager(this);
            graphics.IsFullScreen = Config.FULLSCREEN;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            random = new Random();
            background1 = GetRandomColor();
            background2 = GetRandomColor();
            // uncapped framerate
            //graphics.SynchronizeWithVerticalRetrace = false;
            //this.IsFixedTimeStep = false;
            mapSerializer = new LDTKJsonMapSerializer();
            contentManager = Content;

            Config.CHARACTER_SPEED = 2f;

            frameCounter = new FrameCounter();
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            Entity.GraphicsDeviceManager = graphics;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            camera = new Camera();
            font = Content.Load<SpriteFont>("DefaultFont");
            graphics.PreferredBackBufferWidth = Config.RES_W;
            graphics.PreferredBackBufferHeight = Config.RES_H;
            graphics.ApplyChanges();
            CreateLevel();
            Knight knight = new Knight(Content, new Vector2(5, 5) * Config.GRID, font);
            camera.trackTarget(knight, true);
        }

        private void CreateLevel()
        {

            Scene.Instance.AddScrollableLayer(0.7f, true);
            Scene.Instance.AddScrollableLayer(0.5f, true);

            for (int i = 3; i <= 300; i++)
            {
                if (i % 15 == 0)
                {
                    for (int j = 22; j < 25; j++)
                    {
                        Tile t = new Tile(Content, Scene.Instance.ScrollableBackgroundLayers[1], new Vector2(i * Config.GRID, j * Config.GRID), Color.Yellow, font);
                    }
                }

            }

            for (int i = 3; i <= 300; i++)
            {
                if (i % 20 == 0)
                {
                    for (int j = 18; j < 25; j++)
                    {
                        Tile t = new Tile(Content, Scene.Instance.ScrollableBackgroundLayers[0], new Vector2(i * Config.GRID, j * Config.GRID), Color.Green, font);
                    }
                }
            }

            for (int i = 2; i <= 300; i += 20)
            {
                for (int j = i; j <= i + 5; j++)
                {
                    Tile t = new Tile(Content, Scene.Instance.ColliderLayer, new Vector2(j * Config.GRID, 20 * Config.GRID), Color.Black, font);
                }

            }

            for (int i = 2; i <= 300; i++)
            {
                new Tile(Content, Scene.Instance.ColliderLayer, new Vector2(i * Config.GRID, 25 * Config.GRID), Color.Black, font);
                if (i % 5 == 0)
                {
                    for (int j = i; j < i + 2; j++)
                    {
                        Tile t = new Tile(Content, Scene.Instance.ColliderLayer, new Vector2(j * Config.GRID, 24 * Config.GRID), Color.DarkRed, font);
                    }
                }
            }

            /*LDTKMap map = mapSerializer.Deserialize("D:/GameDev/MonoGame/2DGameEngine/2DGameEngine/Content/practise.json");
            HashSet<Vector2> collisions = map.GetCollisions();
            foreach (Vector2 coord in collisions) {
                new Entity(Scene.Instance.GetColliderLayer(), null , graphics.GraphicsDevice, CreateRectangle(Constants.GRID, Color.Black), coord * Constants.GRID, font);
            }*/
            
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

        private Color GetRandomColor()
        {
            return Color.FromNonPremultiplied(random.Next(256), random.Next(256), random.Next(256), 256);
        }

        protected override void Draw(GameTime gameTime)
        {
            /*elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            sin = (float)Math.Sin(elapsedTime);
            if (sin <= 0.01)
            {
                background2 = GetRandomColor();
                elapsedTime = 0;
            } else if (sin >= 0.99)
            {
                background1 = GetRandomColor();
            }
            
            GraphicsDevice.Clear(Color.Lerp(background1, background2, sin));
            */

            GraphicsDevice.Clear(Color.White);

            // TODO: Add your drawing code here
            RootContainer.Instance.DrawAll(gameTime);

            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            frameCounter.Update(deltaTime);
            var fps = string.Format("FPS: {0}", frameCounter.AverageFramesPerSecond);
            spriteBatch.Begin();
            spriteBatch.DrawString(font, fps, new Vector2(1, 1), Color.Black);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
