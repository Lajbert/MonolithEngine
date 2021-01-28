using ForestPlatformerExample.Source.Enemies;
using ForestPlatformerExample.Source.Environment;
using ForestPlatformerExample.Source.Hero;
using ForestPlatformerExample.Source.Items;
using GameEngine2D.Engine.Source.Entities;
using GameEngine2D.Engine.Source.Graphics;
using GameEngine2D.Engine.Source.Level;
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
using System.Collections.Generic;

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
            Config.GRAVITY_FORCE = 12f;
            Config.ZOOM = 2f;
            Config.CHARACTER_SPEED = 2f;
            Config.JUMP_FORCE = 7f;
            Config.INCREASING_GRAVITY = true;


            //Config.RES_W = 3840;
            //Config.RES_W = 2160;
            //Config.FULLSCREEN = true;

            //Config.GRID = 64;

            //Config.FPS = 0;
            if (Config.FPS == 0)
            {
                // uncapped framerate
                graphics.SynchronizeWithVerticalRetrace = false;
                IsFixedTimeStep = false;
            }
            else
            {
                //Config.FPS = 2000;
                IsFixedTimeStep = true;//false;
                graphics.SynchronizeWithVerticalRetrace = false;
                TargetElapsedTime = TimeSpan.FromSeconds(1d / Config.FPS); //60);
            }
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

            hero = new Hero(new Vector2(18 * Config.GRID, 31 * Config.GRID), font);
            Camera.TrackTarget(hero, true);
            //TODO: use this.Content to load your game content here

            frameCounter = new FrameCounter();

            Logger.Log("Object count: " + GameObject.GetObjectCount());
        }

        private void LoadLevel()
        {
            Dictionary<int, MovingPlatform> platformGroups = new Dictionary<int, MovingPlatform>();

            MapSerializer mapSerializer = new LDTKJsonMapSerializer();
            LDTKMap map = mapSerializer.Deserialize("D:/GameDev/MonoGame/2DGameEngine/ForestPlatformerExample/Maps/level.json");
            foreach (EntityInstance entity in map.entities)
            {
                Vector2 position = new Vector2(entity.Px[0], entity.Px[1]);
                if (entity.Identifier.Equals("Coin"))
                {
                    new Coin(position);
                }
                else if (entity.Identifier.Equals("MovingPlatform"))
                {
                    continue;
                    int group = -1;
                    int travelDistance = 0;
                    foreach (FieldInstance field in entity.FieldInstances)
                    {

                        if (field.Identifier == "group")
                        {
                            group = (int)field.Value;
                        }
                        else if (field.Identifier == "travel_distance")
                        {
                            travelDistance = (int)field.Value;
                        }
                    }
                    if (group == -1 || travelDistance == 0)
                    {
                        throw new Exception("Can't initialize platform group");
                    }
                    if (!platformGroups.ContainsKey(group))
                    {
                        platformGroups.Add(group, new MovingPlatform(travelDistance));
                    }
                    MovingPlatform currentPlatform = platformGroups[group];
                    currentPlatform.AddPlatformElement(position);
                }
                else if (entity.Identifier.Equals("Spring"))
                {
                    int power = -1;
                    foreach (FieldInstance field in entity.FieldInstances)
                    {

                        if (field.Identifier == "power")
                        {
                            power = (int)field.Value;
                        }
                    }
                    Spring spring = new Spring(position, power);
                }
                else if (entity.Identifier.Equals("EnemyCarrot"))
                {
                    int speed = -1;
                    foreach (FieldInstance field in entity.FieldInstances)
                    {

                        if (field.Identifier == "speed")
                        {
                            speed = (int)field.Value;
                        }
                    }
                    Carrot carrot = new Carrot(position, Direction.RIGHT);
                }
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            //gameTime = new GameTime(gameTime.TotalGameTime / 5, gameTime.ElapsedGameTime / 5);
            // TODO: Add your update logic here
            Timer.Update((float)gameTime.ElapsedGameTime.TotalMilliseconds);
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
