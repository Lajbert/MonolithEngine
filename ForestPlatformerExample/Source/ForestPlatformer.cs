using ForestPlatformerExample.Source.Enemies;
using ForestPlatformerExample.Source.Entities.Items;
using ForestPlatformerExample.Source.Environment;
using ForestPlatformerExample.Source.Items;
using ForestPlatformerExample.Source.PlayerCharacter;
using GameEngine2D;
using GameEngine2D.Engine.Source.Entities;
using GameEngine2D.Engine.Source.Graphics;
using GameEngine2D.Engine.Source.Level;
using GameEngine2D.Engine.Source.Physics;
using GameEngine2D.Engine.Source.Physics.Collision;
using GameEngine2D.Engine.Source.Util;
using GameEngine2D.Entities;
using GameEngine2D.Global;
using GameEngine2D.Source.Camera2D;
using GameEngine2D.Source.GridCollision;
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

        private double elapsedTime = 0;
        private GameTime gt = new GameTime();
        private float fixedUpdateRate;

        public ForestPlatformer()
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
            Config.FPS = 144;
            Config.FIXED_UPDATE_FPS = 30;

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

            fixedUpdateRate = Config.FIXED_UPDATE_FPS == 0 ? 0 : (1 / (float)Config.FIXED_UPDATE_FPS) * 1000;
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

            Camera.BOUND_LEFT = 500;
            Camera.BOUND_RIGHT = 2000;
            Camera.BOUND_TOP = 350;
            Camera.BOUND_BOTTOM = 450;

            LayerManager.Instance.Camera = Camera;
            LayerManager.Instance.InitLayers();

            font = Content.Load<SpriteFont>("DefaultFont");

            LoadLevel();

            //TODO: use this.Content to load your game content here

            frameCounter = new FrameCounter();

            Logger.Info("Object count: " + GameObject.GetObjectCount());
        }

        private void LoadLevel()
        {

            MapSerializer mapSerializer = new LDTKJsonMapSerializer();
            LDTKMap map = mapSerializer.Deserialize("D:/GameDev/MonoGame/2DGameEngine/ForestPlatformerExample/Maps/level.json");
            foreach (EntityInstance entity in map.entities)
            {
                Vector2 position = new Vector2(entity.Px[0], entity.Px[1]);
                if (entity.Identifier.Equals("Hero"))
                {

                    hero = new Hero(position, font);
                    Camera.TrackTarget(hero, true);
                }
                else if (entity.Identifier.Equals("Coin"))
                {
                    new Coin(position);
                }
                else if (entity.Identifier.Equals("MovingPlatform"))
                {
                    new MovingPlatform(position, (int)entity.Width, (int)entity.Height);
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
                    Carrot carrot = new Carrot(position, Direction.EAST);
                } else if (entity.Identifier.Equals("Box"))
                {
                    Box box = new Box(position);
                }
                else if (entity.Identifier.Equals("Ladder"))
                {
                    new Ladder(position, (int)entity.Width, (int)entity.Height);
                }
                else if (entity.Identifier.Equals("MovingPlatformTurn"))
                {
                    Direction dir = default(Direction);
                    foreach (FieldInstance field in entity.FieldInstances)
                    {
                        if (field.Identifier == "Direction")
                        {
                            dir = Enum.Parse(typeof(Direction), field.Value);
                        }
                    }
                    new MovingPlatformTurner(position, dir);
                }
            }

            PhysicalEntity collisionTest = new PhysicalEntity(LayerManager.Instance.EntityLayer, null, new Vector2(17, 37) * Config.GRID);
            collisionTest.HasGravity = false;
            collisionTest.AddTag("Mountable");
            //collisionTest.AddComponent(new BoxCollisionComponent(collisionTest, 32, 32, new Vector2(-16, -16)));
            collisionTest.AddComponent(new BoxCollisionComponent(collisionTest, 32, 64, Vector2.Zero));
            (collisionTest.GetCollisionComponent() as BoxCollisionComponent).DEBUG_DISPLAY_COLLISION = true;

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            //gameTime = new GameTime(gameTime.TotalGameTime / 5, gameTime.ElapsedGameTime / 5);
            // TODO: Add your update logic here
            Timer.Update((float)gameTime.ElapsedGameTime.TotalMilliseconds);
            //CollisionEngine.Instance.Update(gameTime);
            LayerManager.Instance.UpdateAll(gameTime);
            Camera.update(gameTime);
            Camera.postUpdate(gameTime);

            if (fixedUpdateRate == 0)
            {
                FixedUpdate(gameTime);
            } else
            {
                if (elapsedTime >= fixedUpdateRate)
                {
                    gt.ElapsedGameTime = TimeSpan.FromMilliseconds(elapsedTime);
                    FixedUpdate(gt);
                    elapsedTime = 0;
                }
                else
                {
                    elapsedTime += gameTime.ElapsedGameTime.TotalMilliseconds;
                }
            }

            

            base.Update(gameTime);
        }

        private void FixedUpdate(GameTime gameTime)
        {
            CollisionEngine.Instance.Update(gameTime);
            LayerManager.Instance.FixedUpdateAll(gameTime);
        }

        private float lastPrint = 0;
        string fps = "";
        protected override void Draw(GameTime gameTime)
        {
            //gameTime = new GameTime(gameTime.TotalGameTime / 5, gameTime.ElapsedGameTime / 5);
            GraphicsDevice.Clear(Color.White);

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
