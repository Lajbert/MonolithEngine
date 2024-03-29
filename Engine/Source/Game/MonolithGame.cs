﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using static MonolithEngine.Camera;

/// <summary>
/// Base class for a game instance. Any new Monolith game should extends
/// this class as an entry point for the game.
/// </summary>
namespace MonolithEngine
{
    public abstract class MonolithGame : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        protected List<Camera> Cameras;

        private FrameCounter frameCounter;

        private int fixedUpdateRate;

        protected SceneManager SceneManager;

        protected CameraMode CameraMode = CameraMode.SINGLE;

        private readonly int DEFAULT_FIXED_UPDATE_RATE = 30;

        private static Platform platform;

        public static Platform Platform
        {
            get => platform;
        }

        public static bool IsGameStarted = false;

#if DEBUG
        private float lastPrint = 0;
        private string fps = "";
#endif

        public MonolithGame(Platform targetPlatform)
        {
            platform = targetPlatform;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Config.GRAVITY_ON = true;
            Config.GRAVITY_FORCE = 0.025f;
            //Config.ZOOM = 2f;
            Config.JUMP_FORCE = 1f;
            Config.INCREASING_GRAVITY = true;

            Config.FIXED_UPDATE_FPS = 30;
            Globals.FixedUpdateMultiplier = 1;
        }

        public void ApplyVideoConfiguration()
        {
            if (VideoConfiguration.FRAME_LIMIT == 0)
            {
                IsFixedTimeStep = false;
            }
            else
            {
                IsFixedTimeStep = true;
                TargetElapsedTime = TimeSpan.FromTicks((long)(TimeSpan.TicksPerSecond / VideoConfiguration.FRAME_LIMIT));
                //TargetElapsedTime = TimeSpan.FromSeconds(1d / Config.FPS); //60);
            }

            if (VideoConfiguration.VSYNC)
            {
                graphics.SynchronizeWithVerticalRetrace = true;
            } 
            else
            {
                graphics.SynchronizeWithVerticalRetrace = false;
            }

            graphics.PreferredBackBufferWidth = VideoConfiguration.RESOLUTION_WIDTH;
            graphics.PreferredBackBufferHeight = VideoConfiguration.RESOLUTION_HEIGHT;
            graphics.IsFullScreen = VideoConfiguration.FULLSCREEN;
            if (Platform.IsMobile())
            {
                Config.SCALE = ((float)VideoConfiguration.RESOLUTION_HEIGHT / 1920) * 2.8f;
            }
            else
            {
                Config.SCALE = ((float)VideoConfiguration.RESOLUTION_WIDTH / 1920) * 2;
            }
            //Config.SCALE = ((float)VideoConfiguration.RESOLUTION_WIDTH / 1920) * 2;
            graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
            graphics.ApplyChanges();
        }

        protected sealed override void Initialize()
        {
            AssetUtil.Content = Content;
            AssetUtil.GraphicsDeviceManager = graphics;
            MonolithTexture.GraphicsDevice = graphics.GraphicsDevice;
            Layer.GraphicsDeviceManager = graphics;
            TileGroup.GraphicsDevice = graphics.GraphicsDevice;
#if DEBUG
            Assets.LoadFont("DebugFont", "Text/InGameText");
#endif
            VideoConfiguration.GameInstance = this;

            Init();
            Logger.Info("Engine initialized with " + Config.FIXED_UPDATE_FPS + " FPS");
            fixedUpdateRate = (int)(Config.FIXED_UPDATE_FPS == 0 ? 0 : (1000 / (float)Config.FIXED_UPDATE_FPS));
            Globals.FixedUpdateRate = TimeSpan.FromTicks((long)(TimeSpan.TicksPerSecond / Config.FIXED_UPDATE_FPS));
            Globals.FixedUpdateMultiplier = (float)DEFAULT_FIXED_UPDATE_RATE / Config.FIXED_UPDATE_FPS;
            //fixedUpdateRate = Config.FIXED_UPDATE_FPS == 0 ? 0 : (float)TimeSpan.FromTicks((long)(TimeSpan.TicksPerSecond / Config.FIXED_UPDATE_FPS)).TotalMilliseconds;
            base.Initialize();
        }

        protected abstract void Init();

        protected sealed override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            if (VideoConfiguration.RESOLUTION_WIDTH == 0 || VideoConfiguration.RESOLUTION_HEIGHT == 0)
            {
                VideoConfiguration.RESOLUTION_WIDTH = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                VideoConfiguration.RESOLUTION_HEIGHT = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            }

            ApplyVideoConfiguration();

            Config.ExitAction = Exit;

            Cameras = new List<Camera>();
            

            if (CameraMode == CameraMode.SINGLE)
            {
                Cameras.Add(new Camera(graphics));
            }
            else
            {
                Cameras.Add(new Camera(graphics, CameraMode, 0));
                Cameras.Add(new Camera(graphics, CameraMode, 1));
            }

            SceneManager = new SceneManager(Cameras, graphics.GraphicsDevice);

            //TODO: use this.Content to load your game content here

            frameCounter = new FrameCounter();

            LoadGameContent();

            if (SceneManager.IsEmpty())
            {
                throw new Exception("No scene added to the game!");
            }

            IsGameStarted = true;
        }

        protected abstract void LoadGameContent();

        private float fixedUpdateElapsedTime = 0;
        private float fixedUpdateDelta = 0.33f;
        private float previousT = 0;
        private float accumulator = 0.0f;
        private float maxFrameTime = 250;

        protected override void Update(GameTime gameTime)
        {
            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            //    Exit();

            if (gameTime.ElapsedGameTime.TotalSeconds > 0.1)
            {
                accumulator = 0;
                previousT = 0;
                return;
            }

            if (previousT == 0)
            {
                fixedUpdateDelta = fixedUpdateRate;
                previousT = (float)gameTime.TotalGameTime.TotalMilliseconds;
            }

            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            Globals.ElapsedTime = elapsedTime;
            Globals.GameTime = gameTime;
            Timer.Update(elapsedTime);
            foreach (Camera cameara in Cameras)
            {
                cameara.Update();
            }

            float now = (float)gameTime.TotalGameTime.TotalMilliseconds;
            float frameTime = now - previousT;
            if (frameTime > maxFrameTime)
                frameTime = maxFrameTime;
            previousT = now;

            accumulator += frameTime;

            while (accumulator >= fixedUpdateDelta)
            {
                FixedUpdate();
                fixedUpdateElapsedTime += fixedUpdateDelta;
                accumulator -= fixedUpdateDelta;
            }

            Globals.FixedUpdateAlpha = (float)(accumulator / fixedUpdateDelta);

            SceneManager.Update();

            base.Update(gameTime);
        }

        protected void FixedUpdate()
        {
            if (Platform == Platform.ANDROID)
            {
                if (GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width != VideoConfiguration.RESOLUTION_WIDTH || GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height != VideoConfiguration.RESOLUTION_HEIGHT)
                {
                    graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                    graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                    VideoConfiguration.RESOLUTION_WIDTH = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                    VideoConfiguration.RESOLUTION_HEIGHT = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                    graphics.ApplyChanges();
                }
            }

            SceneManager.FixedUpdate();
        }

        protected override void Draw(GameTime gameTime)
        {
            //gameTime = new GameTime(gameTime.TotalGameTime / 5, gameTime.ElapsedGameTime / 5);
            SceneManager.Draw(spriteBatch);

#if DEBUG
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            lastPrint += gameTime.ElapsedGameTime.Milliseconds;
            frameCounter.Update(deltaTime);

            if (lastPrint > 10)
            {
                fps = string.Format("FPS: {0}", (int)frameCounter.AverageFramesPerSecond);
                lastPrint = 0;
            }

            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null);
            spriteBatch.DrawString(Assets.GetFont("DebugFont"), fps, new Vector2(1, 100), Color.Red, 0f, default, 3, SpriteEffects.None, 0);
            spriteBatch.End();

#endif

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

    }

    public enum Platform
    {
        MAC_OSX,
        LINUX,
        WINDOWS,
        ANDROID,
        IOS
    }
}
