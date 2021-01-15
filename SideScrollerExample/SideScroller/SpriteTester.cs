using GameEngine2D.Source.Layer;
using GameEngine2D.Engine.Source.Util;
using GameEngine2D.Entities;
using GameEngine2D.GameExamples.SideScroller.Source.Hero;
using GameEngine2D.Global;
using GameEngine2D.Source;
using GameEngine2D.Source.Camera2D;
using GameEngine2D.Source.Level;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SideScrollerExample.SideScroller.Source.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SideScrollerExample.SideScroller
{
    class SpriteTester : Game
    {
        private GraphicsDeviceManager graphics;
        private ContentManager contentManager;
        public static Camera Camera;

        private FrameCounter frameCounter;

        private SpriteBatch spriteBatch;

        //private Camera2D Camera2D;
        //private ResolutionIndependentRenderer resolutionIndependentRenderer;

        private Knight knight;

        private SpriteFont font;

        List<Texture2D> cubes = new List<Texture2D>();

        public SpriteTester()
        {
            // >>>>>>> set framerate >>>>>>>>>>
            //this.IsFixedTimeStep = true;//false;
            //this.TargetElapsedTime = TimeSpan.FromSeconds(1d / Config.FPS); //60);

            graphics = new GraphicsDeviceManager(this);
            graphics.IsFullScreen = Config.FULLSCREEN;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            // uncapped framerate
            graphics.SynchronizeWithVerticalRetrace = false;
            this.IsFixedTimeStep = false;
            contentManager = Content;

            Config.CHARACTER_SPEED = 2f;

            frameCounter = new FrameCounter();
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            SpriteUtil.Content = Content;
            SpriteUtil.GraphicsDeviceManager = graphics;
            Layer2D.GraphicsDeviceManager = graphics;
            LayerManager.Instance.InitLayers();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            //resolutionIndependentRenderer = new ResolutionIndependentRenderer(this);
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("DefaultFont");
            //Camera2D = new Camera2D(resolutionIndependentRenderer);
            //Entity.Camera2D = Camera2D;
            //Entity.ResolutionIndependentRenderer = resolutionIndependentRenderer;
            graphics.PreferredBackBufferWidth = Config.RES_W;
            graphics.PreferredBackBufferHeight = Config.RES_H;
            graphics.ApplyChanges();
            Camera = new Camera(graphics);

            for (int i = 0; i < 5000; i++)
            {
                cubes.Add(SpriteUtil.LoadTexture("SideScroller/KnightAssets/HeroKnight/Fall/HeroKnight_Fall_0"));
            }
        }

       
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //Camera.Position = new Vector2(ms.X, ms.Y);

            // TODO: Add your update logic here
            Camera.update(gameTime);
            Camera.postUpdate(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {

            GraphicsDevice.Clear(Color.White);

            // TODO: Add your drawing code here
            LayerManager.Instance.DrawAll(gameTime);

            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            frameCounter.Update(deltaTime);
            var fps = string.Format("FPS: {0}", frameCounter.AverageFramesPerSecond);
            spriteBatch.Begin();
            foreach (Texture2D sprite in cubes)
            {
                
                spriteBatch.Draw(sprite, new Vector2(500, 500), Color.White);
                
            }
            spriteBatch.End();
            spriteBatch.Begin();
            spriteBatch.DrawString(font, fps, new Vector2(1, 1), Color.Red);
            spriteBatch.End();


            //Camera2D.Position = knight.Position;

            base.Draw(gameTime);
        }
    }
}
