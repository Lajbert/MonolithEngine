using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

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

        protected Camera Camera;

        private SpriteFont font;
        private FrameCounter frameCounter;

        private int fixedUpdateRate;

        protected SceneManager SceneManager;

#if DEBUG
        private SpriteFont debugFont;
#endif

        public MonolithGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Config.GRAVITY_ON = true;
            Config.GRAVITY_FORCE = 0.3f;
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
            } else
            {
                graphics.SynchronizeWithVerticalRetrace = false;
            }

            graphics.PreferredBackBufferWidth = VideoConfiguration.RESOLUTION_WIDTH;
            graphics.PreferredBackBufferHeight = VideoConfiguration.RESOLUTION_HEIGHT;
            graphics.IsFullScreen = VideoConfiguration.FULLSCREEN;
            Config.SCALE = ((float)VideoConfiguration.RESOLUTION_WIDTH / 1920) * 2;
            graphics.ApplyChanges();
        }

        protected sealed override void Initialize()
        {
            AssetUtil.Content = Content;
            AssetUtil.GraphicsDeviceManager = graphics;
            Layer.GraphicsDeviceManager = graphics;
            TileGroup.GraphicsDevice = graphics.GraphicsDevice;
#if DEBUG
            debugFont = Content.Load<SpriteFont>("Fonts/DebugFont");
            Entity.DebugFont = debugFont;
#endif
            VideoConfiguration.GameInstance = this;
            Init();
            fixedUpdateRate = (int)(Config.FIXED_UPDATE_FPS == 0 ? 0 : (1000 / (float)Config.FIXED_UPDATE_FPS));
            Globals.FixedUpdateRate = TimeSpan.FromTicks((long)(TimeSpan.TicksPerSecond / Config.FIXED_UPDATE_FPS));
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

            Camera = new Camera(graphics);

            SceneManager = new SceneManager(Camera, graphics.GraphicsDevice);

            font = Content.Load<SpriteFont>("DefaultFont");

            //TODO: use this.Content to load your game content here

            frameCounter = new FrameCounter();

            LoadGameContent();

            if (SceneManager.IsEmpty())
            {
                throw new Exception("No scene added to the game!");
            }
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
            Camera.Update();

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
            SceneManager.FixedUpdate();
        }

        private float lastPrint = 0;
        string fps = "";
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

            spriteBatch.Begin();
            spriteBatch.DrawString(font, fps, new Vector2(1, 100), Color.Red);
            spriteBatch.End();

#endif

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

    }
}
