using ForestPlatformerExample.Source.Hero;
using ForestPlatformerExample.Source.Items;
using GameEngine2D.Engine.Source.Graphics;
using GameEngine2D.Engine.Source.Util;
using GameEngine2D.Entities;
using GameEngine2D.Global;
using GameEngine2D.Source.Camera2D;
using GameEngine2D.Source.Layer;
using GameEngine2D.Source.Level;
using GameEngine2D.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace ForestPlatformerExample
{
    public class ForestPlatformer : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private Camera Camera;

        private SpriteFont font;
        private FrameCounter frameCounter;

        private Hero hero;

        public ForestPlatformer()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Config.GRAVITY_ON = true;
            Config.GRAVITY_FORCE = 16f;
            Config.ZOOM = 2f;
            Config.CHARACTER_SPEED = 3f;
            Config.JUMP_FORCE = 7f;

            //Config.RES_W = 3840;
            //Config.RES_W = 2160;
            //Config.FULLSCREEN = true;

            //Config.GRID = 64;

           //this.IsFixedTimeStep = true;//false;
           //this.TargetElapsedTime = TimeSpan.FromSeconds(1d / Config.FPS); //60);

            // uncapped framerate
            graphics.SynchronizeWithVerticalRetrace = false;
            this.IsFixedTimeStep = false;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            SpriteUtil.Content = Content;
            SpriteUtil.GraphicsDeviceManager = graphics;
            Layer.GraphicsDeviceManager = graphics;
            TileGroup.GraphicsDevice = graphics.GraphicsDevice;
            //font = Content.Load<SpriteFont>("DefaultFont");
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            graphics.PreferredBackBufferWidth = Config.RES_W;
            graphics.PreferredBackBufferHeight = Config.RES_H;
            graphics.IsFullScreen = Config.FULLSCREEN;
            graphics.ApplyChanges();
            Camera = new Camera(graphics);
            LayerManager.Instance.Camera = Camera;
            LayerManager.Instance.InitLayers();

            font = Content.Load<SpriteFont>("DefaultFont");

            LoadLevel();

            hero = new Hero(new Vector2(300, 300), font);
            Camera.TrackTarget(hero, true);
            // TODO: use this.Content to load your game content here

            frameCounter = new FrameCounter();

            Logger.Log("Object count: " + GameObject.GetObjectCount());
        }

        private void LoadLevel()
        {
            MapSerializer mapSerializer = new LDTKJsonMapSerializer();
            LDTKMap map = mapSerializer.Deserialize("D:/GameDev/MonoGame/2DGameEngine/ForestPlatformerExample/Maps/level.json");
            foreach ((string, Vector2) entity in map.entities)
            {
                if (entity.Item1.Equals("Coin"))
                {
                    new Coin(entity.Item2);
                }
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            //gameTime = new GameTime(gameTime.TotalGameTime / 5, gameTime.ElapsedGameTime / 5);
            // TODO: Add your update logic here
            LayerManager.Instance.UpdateAll(gameTime);
            Camera.update(gameTime);
            Camera.postUpdate(gameTime);

            base.Update(gameTime);
        }

        private float lastPrint = 0;
        string fps = "";
        protected override void Draw(GameTime gameTime)
        {
            //gameTime = new GameTime(gameTime.TotalGameTime / 5, gameTime.ElapsedGameTime / 5);
            GraphicsDevice.Clear(Color.CornflowerBlue);

            lastPrint += gameTime.ElapsedGameTime.Milliseconds;
            LayerManager.Instance.DrawAll(gameTime);

            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            frameCounter.Update(deltaTime);
            
            if (lastPrint > 10)
            {
                fps = string.Format("FPS: {0}", frameCounter.AverageFramesPerSecond);
                lastPrint = 0;
            }

            spriteBatch.Begin();
            spriteBatch.DrawString(font, fps, new Vector2(1, 1), Color.Red);
            spriteBatch.End();


            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
