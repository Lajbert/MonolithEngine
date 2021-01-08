using GameEngine2D.Engine.Source.Graphics.Primitives;
using GameEngine2D.Engine.Source.Util;
using GameEngine2D.Entities;
using GameEngine2D.Global;
using GameEngine2D.Source;
using GameEngine2D.Source.Camera;
using GameEngine2D.Source.Entities.Animation;
using GameEngine2D.Source.Level;
using GameEngine2D.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.GameExamples2D.SideScroller.src
{
    public class LightsDemo : Game
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

        Effect tutorialEffect;

        Texture2D rect;

        Texture2D lightMask;

        RenderTarget2D lightsTarget;
        RenderTarget2D mainTarget;

        public LightsDemo()
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

            frameCounter = new FrameCounter();

            Config.CHARACTER_SPEED = 2f;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            SpriteUtil.GraphicsDeviceManager = graphics;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            camera = new Camera();
            font = Content.Load<SpriteFont>("DefaultFont");
            tutorialEffect = Content.Load<Effect>("Shaders/Effects/Tutorial");
            /*Entity child = new Entity(hero, graphics.GraphicsDevice, CreateRectangle(Constants.GRID, Color.Black), new Vector2(1 , 0) * Constants.GRID, font);
            Entity child2 = new Entity(child, graphics.GraphicsDevice, CreateRectangle(Constants.GRID, Color.Red), new Vector2(1, 0) * Constants.GRID, font);
            Entity child3 = new Entity(child2, graphics.GraphicsDevice, CreateRectangle(Constants.GRID, Color.Blue), new Vector2(1, 0) * Constants.GRID, font);*/
            // TODO: use this.Content to load your game content here
            graphics.PreferredBackBufferWidth = Config.RES_W;
            graphics.PreferredBackBufferHeight = Config.RES_H;
            graphics.ApplyChanges();
            CreateLevel();
            //public Knight(GraphicsLayer layer, Entity parent, GraphicsDeviceManager graphicsDevice, ContentManager content, SpriteBatch spriteBatch, Vector2 position, SpriteFont font)

            rect = SpriteUtil.CreateRectangle(Config.GRID, Color.White);

            lightMask = Content.Load<Texture2D>("Shaders/Textures/Lightmask");

            PresentationParameters pp = GraphicsDevice.PresentationParameters;
            lightsTarget = new RenderTarget2D(
                GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);
            mainTarget = new RenderTarget2D(
                GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);
        }

        private void CreateLevel()
        {



            for (int i = 2; i <= 300; i++)
            {
                Entity level = new Entity(Scene.Instance.ColliderLayer, null, new Vector2(i * Config.GRID, 25 * Config.GRID), font);
                level.SetSprite(SpriteUtil.CreateRectangle(Config.GRID, GetRandomColor()));
            }

            for (int i = 3; i <= 300; i++)
            {
                if (i % 15 == 0)
                {
                    for (int j = 22; j < 25; j++)
                    {
                        Entity level = new Entity(RootContainer.Instance.AddScrollableLayer(0, 0.7f, true), null, new Vector2(i * Config.GRID, j * Config.GRID), font);
                        level.SetSprite(SpriteUtil.CreateRectangle(Config.GRID, Color.Brown));
                    }
                }

            }

            for (int i = 3; i <= 300; i++)
            {
                if (i % 20 == 0)
                {
                    for (int j = 18; j < 25; j++)
                    {
                        Entity level = new Entity(RootContainer.Instance.AddScrollableLayer(0, 0.5f, true), null, new Vector2(i * Config.GRID, j * Config.GRID), font);
                        level.SetSprite(SpriteUtil.CreateRectangle(Config.GRID, Color.Black));
                    }
                }
            }

            for (int i = 2; i <= 300; i += 20)
            {
                for (int j = i; j <= i + 5; j++)
                {
                    Entity level = new Entity(Scene.Instance.ColliderLayer, null, new Vector2(j * Config.GRID, 20 * Config.GRID), font);
                    level.SetSprite(SpriteUtil.CreateRectangle(Config.GRID, GetRandomColor()));
                }

            }
            /*LDTKMap map = mapSerializer.Deserialize("D:/GameDev/MonoGame/2DGameEngine/2DGameEngine/Content/practise.json");
            HashSet<Vector2> collisions = map.GetCollisions();
            foreach (Vector2 coord in collisions) {
                new Entity(Scene.Instance.GetColliderLayer(), null , graphics.GraphicsDevice, CreateRectangle(Constants.GRID, Color.Black), coord * Constants.GRID, font);
            }*/
            /*
            for (int i = 2 * Constants.GRID; i < 15 * Constants.GRID; i += Constants.GRID)
            {
                new Entity(null, graphics.GraphicsDevice, CreateRectangle(Constants.GRID, Color.Black), new Vector2(i, 17 * Constants.GRID), font);
            }

            for (int i = 16 * Constants.GRID; i < 27 * Constants.GRID; i += Constants.GRID)
            {
                new Entity(null, graphics.GraphicsDevice, CreateRectangle(Constants.GRID, Color.Black), new Vector2(i, 15 * Constants.GRID), font);
            }

            for (int i = 2 * Constants.GRID; i < 25 * Constants.GRID; i+= Constants.GRID)
            {
                new Entity(null, graphics.GraphicsDevice, CreateRectangle(Constants.GRID, Color.Black), new Vector2(i, 20 * Constants.GRID), font);
            }

            for (int i = 9 * Constants.GRID; i < 10 * Constants.GRID; i += Constants.GRID)
            {
                new Entity(null, graphics.GraphicsDevice, CreateRectangle(Constants.GRID, Color.Black), new Vector2(i, 19 * Constants.GRID), font);
            }

            for (int i = 25 * Constants.GRID; i < 50 * Constants.GRID; i += Constants.GRID)
            {
                new Entity(null, graphics.GraphicsDevice, CreateRectangle(Constants.GRID, Color.Black), new Vector2(i, 19 * Constants.GRID), font);
            }*/
        }

        float angle = 0;
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            RootContainer.Instance.UpdateAll(gameTime);
            camera.update(gameTime);
            camera.postUpdate(gameTime);

            angle += 0.025f;

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

            /*spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            tutorialEffect.CurrentTechnique.Passes[0].Apply();
            spriteBatch.Draw(rect, new Vector2(300,300), Color.White);
            spriteBatch.End();*/

            GraphicsDevice.SetRenderTarget(lightsTarget);
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);
            spriteBatch.Draw(lightMask, new Vector2(0, 0), Color.Blue);
            spriteBatch.Draw(lightMask, new Vector2(100, 0), Color.Red);
            spriteBatch.Draw(lightMask, new Vector2(200, 200), Color.White);
            spriteBatch.Draw(lightMask, new Vector2(300, 300), Color.Yellow);
            spriteBatch.Draw(lightMask, new Vector2(500, 200), Color.Orange);
            spriteBatch.End();

            // Draw the main scene to the Render Target
            GraphicsDevice.SetRenderTarget(mainTarget);
            GraphicsDevice.Clear(Color.White);
            spriteBatch.Begin();
            spriteBatch.Draw(rect, new Vector2(100, 0), Color.Red);
            spriteBatch.Draw(rect, new Vector2(250, 250), Color.Red);
            Vector2 pos = new Vector2((float)Math.Sin(angle) * 100, (float)Math.Cos(angle) * 100)  + new Vector2(400,400);
            spriteBatch.Draw(rect, pos, Color.Red);
            //spriteBatch.Draw(rect, new Vector2(550, 225), Color.Red);
            spriteBatch.End();

            // Draw the main scene with a pixel shader
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            tutorialEffect.Parameters["lightMask"].SetValue(lightsTarget);
            tutorialEffect.CurrentTechnique.Passes[0].Apply();
            spriteBatch.Draw(mainTarget, Vector2.Zero, Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
