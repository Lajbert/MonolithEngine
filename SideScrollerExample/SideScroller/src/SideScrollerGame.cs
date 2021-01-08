using GameEngine2D.Engine.Source.Camera;
using GameEngine2D.Engine.Source.Global;
using GameEngine2D.Source.Layer;
using GameEngine2D.Engine.Source.Util;
using GameEngine2D.Entities;
using GameEngine2D.GameExamples.SideScroller.Source.Hero;
using GameEngine2D.Global;
using GameEngine2D.Source;
using GameEngine2D.Source.Camera;
using GameEngine2D.Source.Level;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SideScrollerExample.SideScroller.Source.Entities;
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
        public static Camera Camera;
        private Random random;
        private Color background1;
        private Color background2;
        private float sin;
        private MapSerializer mapSerializer;
        private float elapsedTime = 0;

        private FrameCounter frameCounter;

        private SpriteBatch spriteBatch;

        //private Camera2D Camera2D;
        //private ResolutionIndependentRenderer resolutionIndependentRenderer;

        private Knight knight;

        public SideScrollerGame()
        {
            // >>>>>>> set framerate >>>>>>>>>>
            //this.IsFixedTimeStep = true;//false;
            //this.TargetElapsedTime = TimeSpan.FromSeconds(1d / Config.FPS); //60);

            graphics = new GraphicsDeviceManager(this);
            graphics.IsFullScreen = Config.FULLSCREEN;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            random = new Random();
            background1 = SpriteUtil.GetRandomColor();
            background2 = SpriteUtil.GetRandomColor();
            // uncapped framerate
            graphics.SynchronizeWithVerticalRetrace = false;
            this.IsFixedTimeStep = false;
            mapSerializer = new LDTKJsonMapSerializer();
            contentManager = Content;

            Config.CHARACTER_SPEED = 2f;

            frameCounter = new FrameCounter();
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            SpriteUtil.Content = Content;
            SpriteUtil.GraphicsDeviceManager = graphics;
            Layer.GraphicsDeviceManager = graphics;
            RootContainer.Instance.InitLayers();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            //resolutionIndependentRenderer = new ResolutionIndependentRenderer(this);
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Camera = new Camera();
            //Camera2D = new Camera2D(resolutionIndependentRenderer);
            //Entity.Camera2D = Camera2D;
            //Entity.ResolutionIndependentRenderer = resolutionIndependentRenderer;
            Globals.Camera = Camera;
            font = Content.Load<SpriteFont>("DefaultFont");
            graphics.PreferredBackBufferWidth = Config.RES_W;
            graphics.PreferredBackBufferHeight = Config.RES_H;
            graphics.ApplyChanges();
            Camera.Position = Vector2.Zero;
            CreateLevel();
            knight = new Knight(Content, new Vector2(350, 0), font);
            //Camera.trackTarget(knight, true);
        }

        private void CreateLevel()
        {
            LDTKMap map = mapSerializer.Deserialize("D:/GameDev/LDTK levels/practise/practise3_pivot.json");

             /*for (int i = 3; i <= 300; i++)
             {
                 if (i % 15 == 0)
                 {
                     for (int j = 22; j < 25; j++)
                     {
                        Tile t = new Tile(RootContainer.Instance.AddScrollableLayer(0, 0.5f), new Vector2(i * Config.GRID, j * Config.GRID), Color.Yellow, false, font);
                     }
                 }

             }

             for (int i = 3; i <= 300; i++)
             {
                 if (i % 20 == 0)
                 {
                     for (int j = 18; j < 25; j++)
                     {
                        Tile t = new Tile(RootContainer.Instance.AddScrollableLayer(0, 0.7f), new Vector2(i * Config.GRID, j * Config.GRID), Color.Green, false, font);
                    }
                 }
             }

             for (int i = 2; i <= 300; i += 20)
             {
                 for (int j = i; j <= i + 5; j++)
                 {
                    Tile t = new Tile(RootContainer.Instance.EntityLayer, new Vector2(j * Config.GRID, 20 * Config.GRID), Color.Black, false, font);
                    t.HasCollision = true;
                 }

             }

             for (int i = 2; i <= 300; i++)
             {
                 new Tile(RootContainer.Instance.EntityLayer, new Vector2(i * Config.GRID, 25 * Config.GRID), Color.Black, true, font);
                 if (i % 5 == 0)
                 {
                     for (int j = i; j < i + 2; j++)
                     {
                        Tile t = new Tile(RootContainer.Instance.EntityLayer, new Vector2(j * Config.GRID, 24 * Config.GRID), Color.DarkRed, true, font);
                     }
                 }

                 if (i % 4 == 0)
                 {
                     for (int j = 14; j < 19; j++)
                     {
                        Tile t = new Tile(RootContainer.Instance.EntityLayer, new Vector2(i * Config.GRID, j * Config.GRID), Color.Green, true, font);
                    }
                 }
             }*/

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

            MouseState ms = Mouse.GetState();
            //Camera.Position = new Vector2(ms.X, ms.Y);

            // TODO: Add your update logic here
            RootContainer.Instance.UpdateAll(gameTime);
            Camera.update(gameTime);
            Camera.postUpdate(gameTime);
            base.Update(gameTime);
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
            spriteBatch.DrawString(font, fps, new Vector2(1, 1), Color.Red);
            spriteBatch.End();

            //Camera2D.Position = knight.Position;

            base.Draw(gameTime);
        }
    }
}
