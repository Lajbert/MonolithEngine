using GameEngine2D.Engine.Source.Graphics.Primitives;
using GameEngine2D.Engine.Source.Physics.Raycast;
using GameEngine2D.Engine.Source.Util;
using GameEngine2D.Entities;
using GameEngine2D.GameExamples.TopDown.Source.Hero;
using GameEngine2D.Global;
using GameEngine2D.Source;
using GameEngine2D.Source.Camera2D;
using GameEngine2D.Source.Layer;
using GameEngine2D.Source.Level;
using GameEngine2D.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace TopDownExample
{
    class TopDownGame : Game
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
        private SpriteBatch spriteBatch;
        private Line line;
        private Line line2;
        private Line line3;
        private Line line4;
        private Line line5;
        private Vector2 intersection = Vector2.Zero;
        private FrameCounter frameCounter;

        public TopDownGame()
        {
            // >>>>>>> set framerate >>>>>>>>>>
            //this.IsFixedTimeStep = true;//false;
            //this.TargetElapsedTime = TimeSpan.FromSeconds(1d / Config.FPS); //60);

            graphics = new GraphicsDeviceManager(this);
            graphics.IsFullScreen = Config.FULLSCREEN;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            random = new Random();
            background1 = GetRandomColor();
            background2 = GetRandomColor();

            // uncapped framerate
            graphics.SynchronizeWithVerticalRetrace = false;
            this.IsFixedTimeStep = false;

            mapSerializer = new LDTKJsonMapSerializer();
            contentManager = Content;

            frameCounter = new FrameCounter();

            Config.GRAVITY_ON = false;
            Config.CHARACTER_SPEED = 2f;
            Config.GRID = 64;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            Layer2D.GraphicsDeviceManager = graphics;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("DefaultFont");
            graphics.PreferredBackBufferWidth = Config.RES_W;
            graphics.PreferredBackBufferHeight = Config.RES_H;
            graphics.ApplyChanges();
            camera = new Camera(graphics);
            LayerManager.Instance.Camera = camera;
            LayerManager.Instance.InitLayers();
            SpriteUtil.GraphicsDeviceManager = graphics;
            SpriteUtil.Content = Content;

            CubeGuy cube = new CubeGuy(new Vector2(5, 5) * Config.GRID, font);
            camera.TrackTarget(cube, true);
            new Entity(LayerManager.Instance.EntityLayer, null, new Vector2(5, 5) * Config.GRID, SpriteUtil.CreateCircle(Config.GRID, Color.Red));

            Entity e = new Entity(LayerManager.Instance.EntityLayer, null, new Vector2(10, 5) * Config.GRID, SpriteUtil.CreateRectangle(Config.GRID, Color.Red), false);
            //e.BlocksRay = true;

            //LDTKMap map = mapSerializer.Deserialize("D:/GameDev/LDTK levels/practise/practise.json");

            //LDTKMap map = mapSerializer.Deserialize("D:/GameDev/LDTK levels/practise/practise3_pivot.json");
            /*map.LoadMap();
            HashSet<Vector2> colliders = map.Colliders;*/
            /*foreach (Vector2 coord in colliders)
            {
                Entity e = new Entity(Scene.Instance.ColliderLayer, null, coord * Config.GRID, font);
                e.SetSprite(SpriteUtil.CreateRectangle(Config.GRID, Color.White));
                e.BlocksRay = true;

            }*/

            //camera.LevelGridCountW = map.worldGridWidth;
            //camera.LevelGridCountH = map.worldGridHeight;

            Ray2DEmitter emitter = new Ray2DEmitter(cube);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            LayerManager.Instance.UpdateAll(gameTime);
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

            MouseState ms = Mouse.GetState();
            if (ms.LeftButton == ButtonState.Pressed)
            {
                //Logger.Log("Mouse x,y: " + ms.X + " " + ms.Y);
                //Logger.Log("Mouse corrected x,y: " + (ms.X + Scene.Instance.GetEntityLayer().GetPosition().X) +  " " + (ms.Y + Scene.Instance.GetEntityLayer().GetPosition().Y));
                //new Circle(null, new Vector2(ms.X, ms.Y), 10, Color.White);
            }
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

            GraphicsDevice.Clear(Color.CornflowerBlue);
            //intersection = ray.Cast((line.GetRayBlockerLines()[0].start - Scene.Instance.GetEntityLayer().GetPosition(), line.GetRayBlockerLines()[0].end - Scene.Instance.GetEntityLayer().GetPosition()));
            //intersection = ray.Cast(line.GetRayBlockerLines()[0]);
            //ray.position = new Vector2(300, 300);
            //if (intersection != Vector2.Zero)
            //{
                //marker.SetPosition(intersection);
                //Logger.Log("Cast: :" + intersection);
            //}

            // TODO: Add your drawing code here
            LayerManager.Instance.DrawAll(gameTime);
            //Logger.Log("ENTITY LAYER: " + Scene.Instance.GetEntityLayer().GetPosition());


            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            frameCounter.Update(deltaTime);
            var fps = string.Format("FPS: {0}", frameCounter.AverageFramesPerSecond);
            spriteBatch.Begin();
            spriteBatch.DrawString(font, fps, new Vector2(1, 1), Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
