using MonolithEngine.Engine.Source.Global;
using MonolithEngine.Engine.Source.Graphics;
using MonolithEngine.Engine.Source.Physics;
using MonolithEngine.Engine.Source.Util;
using MonolithEngine.Entities;
using MonolithEngine.Global;
using MonolithEngine.Source.Camera2D;
using MonolithEngine.Source.GridCollision;
using MonolithEngine.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonolithEngine.Engine.Source.MyGame
{
    public abstract class MonolithGame : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private Camera Camera;

        private SpriteFont font;
        private FrameCounter frameCounter;

        private int fixedUpdateRate;

        public static int CoinCount = 0;

        public MonolithGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Config.GRAVITY_ON = true;
            Config.GRAVITY_FORCE = 12f;
            //Config.ZOOM = 2f;
            Config.CHARACTER_SPEED = 2f;
            Config.JUMP_FORCE = 7f;
            Config.INCREASING_GRAVITY = true;


            Config.RES_W = 3840;
            Config.RES_H = 2160;
            //Config.FULLSCREEN = true;
            Config.ZOOM = (Config.RES_W / 1920) * 2;
            Config.FPS = 600;
            Config.FIXED_UPDATE_FPS = 30;

            if (Config.FPS == 0)
            {
                // uncapped framerate
                //graphics.SynchronizeWithVerticalRetrace = true;
                graphics.SynchronizeWithVerticalRetrace = false;
                IsFixedTimeStep = false;
            }
            else
            {
                IsFixedTimeStep = true;//false;
                graphics.SynchronizeWithVerticalRetrace = true;
                //graphics.SynchronizeWithVerticalRetrace = false;
                TargetElapsedTime = TimeSpan.FromTicks((long)(TimeSpan.TicksPerSecond / Config.FPS));
                //TargetElapsedTime = TimeSpan.FromSeconds(1d / Config.FPS); //60);
            }

            fixedUpdateRate = (int)(Config.FIXED_UPDATE_FPS == 0 ? 0 : (1000 / (float)Config.FIXED_UPDATE_FPS));
            //fixedUpdateRate = Config.FIXED_UPDATE_FPS == 0 ? 0 : (float)TimeSpan.FromTicks((long)(TimeSpan.TicksPerSecond / Config.FIXED_UPDATE_FPS)).TotalMilliseconds;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            TextureUtil.Content = Content;
            TextureUtil.GraphicsDeviceManager = graphics;
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
            Camera = new Camera(graphics)
            {
                BOUND_LEFT = 500,
                BOUND_RIGHT = 2000,
                BOUND_TOP = 350,
                BOUND_BOTTOM = 450
            };

            LayerManager.Instance.Camera = Camera;
            LayerManager.Instance.InitLayers();

            font = Content.Load<SpriteFont>("DefaultFont");

            //TODO: use this.Content to load your game content here

            frameCounter = new FrameCounter();

            Logger.Info("Object count: " + GameObject.GetObjectCount());
        }

        private float fixedUpdateElapsedTime = 0;
        private float fixedUpdateDelta = 0.33f;
        private float previousT = 0;
        private float accumulator = 0.0f;
        private float maxFrameTime = 250;

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (previousT == 0)
            {
                fixedUpdateDelta = fixedUpdateRate;
                previousT = (float)gameTime.TotalGameTime.TotalMilliseconds;
            }

            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            Globals.ElapsedTime = elapsedTime;
            Globals.GameTime = gameTime;
            Timer.Update(elapsedTime);
            Camera.Update();
            Camera.PostUpdate();

            float now = (float)gameTime.TotalGameTime.TotalMilliseconds;
            float frameTime = now - previousT;
            if (frameTime > maxFrameTime)
                frameTime = maxFrameTime;
            previousT = now;

            accumulator += frameTime;

            while (accumulator >= fixedUpdateDelta)
            {
                Globals.FixedUpdateMultiplier = 30;
                FixedUpdate();
                fixedUpdateElapsedTime += fixedUpdateDelta;
                accumulator -= fixedUpdateDelta;
            }

            Globals.FixedUpdateAlpha = (float)(accumulator / fixedUpdateDelta);

            LayerManager.Instance.UpdateAll();

            base.Update(gameTime);
        }

        private void FixedUpdate()
        {
            LayerManager.Instance.FixedUpdateAll();
            CollisionEngine.Instance.Update();
        }

        private float lastPrint = 0;
        string fps = "";
        protected override void Draw(GameTime gameTime)
        {
            //gameTime = new GameTime(gameTime.TotalGameTime / 5, gameTime.ElapsedGameTime / 5);
            GraphicsDevice.Clear(Color.White);

            LayerManager.Instance.DrawAll(gameTime);

            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            lastPrint += gameTime.ElapsedGameTime.Milliseconds;
            frameCounter.Update(deltaTime);

            if (lastPrint > 10)
            {
                fps = string.Format("FPS: {0}", frameCounter.AverageFramesPerSecond);
                lastPrint = 0;
            }

            spriteBatch.Begin();
            spriteBatch.DrawString(font, fps, new Vector2(1, 100), Color.Red);
            spriteBatch.End();


            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
