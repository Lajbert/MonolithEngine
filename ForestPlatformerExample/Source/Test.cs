using GameEngine2D.Engine.Source.Entities;
using GameEngine2D.Engine.Source.Graphics;
using GameEngine2D.Engine.Source.Level;
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
using ForestPlatformerExample.Source.PlayerCharacter;
using ForestPlatformerExample.Source.Environment;
using ForestPlatformerExample.Source.Items;
using ForestPlatformerExample.Source.Enemies;
using GameEngine2D.Engine.Source.Physics.Collision;
using System.Linq;
using GameEngine2D.Source.Util;
using ForestPlatformerExample.Source.Weapons;
using GameEngine2D;
using GameEngine2D.Engine.Source.Physics.Interface;
using GameEngine2D.Engine.Source.Physics;
using GameEngine2D.Engine.Source.Physics.Bresenham;
using GameEngine2D.Engine.Source.Physics.Trigger;
using GameEngine2D.Engine.Source.Entities.Abstract;
using GameEngine2D.Engine.Source.Entities.Animations;

namespace TestExample
{
    public class Test : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private Camera Camera;

        private SpriteFont font;
        private FrameCounter frameCounter;

        private HeroTest hero;
        private EntityTest e;
        private LineEntity line;
        private BoxTrigger bt;

        private List<Vector2> lineToDraw = new List<Vector2>();

        public Test()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Config.GRAVITY_ON = false;
            Config.GRAVITY_FORCE = 12f;
            Config.ZOOM = 2f;
            Config.CHARACTER_SPEED = 2f;
            Config.JUMP_FORCE = 7f;
            Config.INCREASING_GRAVITY = true;


            Config.RES_W = 3840;
            Config.RES_H = 2160;
            //Config.FULLSCREEN = true;
            Config.ZOOM = (Config.RES_W / 1920) * 2;

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

            LoadLevel();

            hero = new HeroTest(font);
            //hero.SetSprite(SpriteUtil.CreateRectangle(16, Color.Blue));

            e = new EntityTest();
            bt = new BoxTrigger(100, 100, new Vector2(-50, -50), showTrigger: false);
            e.AddComponent(bt);
            bt.DEBUG_DISPLAY_TRIGGER = true;

            line = new LineEntity(hero, e);

            Camera.TrackTarget(hero, true);
            //TODO: use this.Content to load your game content here

            frameCounter = new FrameCounter();

            Logger.Debug("Object count: " + GameObject.GetObjectCount());
        }

        class FistTest : Fist
        {
            private Texture2D red = TextureUtil.CreateRectangle(16, Color.Red);
            private Texture2D blue = TextureUtil.CreateRectangle(16, Color.Blue);

            public FistTest(Entity parent, Vector2 position) : base(parent, position)
            {

            }

            /*protected override void OnCircleCollisionStart(Entity otherCollider, CollisionResult collisionResult)
            {
                SetSprite(red);
            }*/

            /*protected override void OnCircleCollisionEnd(Entity otherCollider)
            {
                SetSprite(blue);
            }*/

            public override void Update(GameTime gameTime)
            {
            }
        }

        class HeroTest : Hero
        {

            private Texture2D red = TextureUtil.CreateRectangle(16, Color.Red);
            private Texture2D blue = TextureUtil.CreateRectangle(16, Color.Blue);

            public HeroTest(SpriteFont font) : base(new Vector2(18 * Config.GRID, 31 * Config.GRID), font)
            {
                DEBUG_SHOW_PIVOT = true;
                RemoveComponent<AnimationStateMachine>();
                DEBUG_SHOW_COLLIDER = true;
                //CircleCollider = new CircleCollider(this, 30);
                //fist = new FistTest(this, new Vector2(20, 20));
                /*fist.Destroy();
                fist = new FistTest(this, Vector2.Zero);
                fist.SetSprite(SpriteUtil.CreateRectangle(Config.GRID, Color.Black));
                fist.EnableCircleCollisions = true;
                fist.Active = true;*/

                AddCollisionAgainst("Test");
                //CollisionComponent = new CircleCollisionComponent(this, 30);
                ICollisionComponent CollisionComponent = new BoxCollisionComponent(this, 20, 20, new Vector2(-10, -10));
                (CollisionComponent as AbstractCollisionComponent).DEBUG_DISPLAY_COLLISION = true;
                AddComponent(CollisionComponent);
                //DrawOffset = new Vector2(-8, -8);

                CanFireTriggers = true;
            }

            float angle;
            bool colliding = false;
            /*protected override void OnCircleCollisionStart(Entity otherCollider, CollisionResult collisionResult)
            {
                collisionResult.ApplyRepel(5);
            }*/

            /*protected override void OnCircleCollisionEnd(Entity otherCollider)
            {
                //SetSprite(blue);
                colliding = false;
            }*/

            public override void OnCollisionStart(IGameObject otherCollider)
            {
                PhysicsUtil.ApplyRepel(this, otherCollider as IColliderEntity, 5);
                base.OnCollisionStart(otherCollider);
            }

            Texture2D collPivot = TextureUtil.CreateCircle(5, Color.Black);
            public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
            {
                base.Draw(spriteBatch, gameTime);
                //spriteBatch.Draw(collPivot, CircleCollider.Position, Color.White);
                //if (colliding)
                    //spriteBatch.DrawString(font, "Angle: " + angle, Position, Color.Black);

            }

            public override void Update(GameTime gameTime)
            {
                base.Update(gameTime);
            }
        }

        class EntityTest: PhysicalEntity
        {
            public EntityTest() : base(LayerManager.Instance.EntityLayer, null, new Vector2(22 * Config.GRID, 33 * Config.GRID))
            {
                //CollisionComponent = new CircleCollisionComponent(this, 30);
                ICollisionComponent CollisionComponent = new BoxCollisionComponent(this, 20, 20, new Vector2(-10, -10));
                (CollisionComponent as AbstractCollisionComponent).DEBUG_DISPLAY_COLLISION = true;
                AddComponent(CollisionComponent);
                SetSprite(TextureUtil.CreateRectangle(16, Color.Green));
                //(CollisionComponent as AbstractCollisionComponent).DEBUG_DISPLAY_COLLISION = true;
                DEBUG_SHOW_PIVOT = true;
                DEBUG_SHOW_COLLIDER = true;
                //DrawOffset = new Vector2(-8, -8);
                AddTag("Test");
            }

            Texture2D collPivot = TextureUtil.CreateCircle(5, Color.Black);
            public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
            {
                base.Draw(spriteBatch, gameTime);
                spriteBatch.Draw(collPivot, GetComponent<ICollisionComponent>().Position, Color.White);
            }

            public override void OnEnterTrigger(string triggerTag, IGameObject otherEntity)
            {
                Logger.Info("TRIGGER ENTERED: " + triggerTag);
                base.OnEnterTrigger(triggerTag, otherEntity);
            }

            public override void OnLeaveTrigger(string triggerTag, IGameObject otherEntity)
            {
                Logger.Info("TRIGGER ENDED: " + triggerTag);
                base.OnLeaveTrigger(triggerTag, otherEntity);
            }
        }

        class LineEntity : Entity
        {
            List<Vector2> line = new List<Vector2>();
            Entity other;
            public LineEntity(Entity owner, Entity other) : base(LayerManager.Instance.EntityLayer, owner, Vector2.Zero)
            {
                this.other = other;
                Active = true;
            }

            bool canRayPass;
            public override void Update(GameTime gameTime)
            {
                base.Update(gameTime);
                line.Clear();
                Bresenham.GetLine(Parent.Transform.Position, other.Transform.Position, line);
                canRayPass = Bresenham.CanLinePass(Parent.Transform.Position, other.Transform.Position, (x, y) => {
                    return GridCollisionChecker.Instance.HasBlockingColliderAt(new Vector2(x / Config.GRID, y / Config.GRID), Direction.CENTER);
                });
            }

            public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
            {
                base.Draw(spriteBatch, gameTime);
                if (canRayPass)
                {
                    foreach (Vector2 point in line)
                    {
                        spriteBatch.Draw(TextureUtil.CreateRectangle(1, Color.Black), point, Color.White);
                    }
                } else
                {
                    foreach (Vector2 point in line)
                    {
                        spriteBatch.Draw(TextureUtil.CreateRectangle(1, Color.Red), point, Color.White);
                    }
                }
                
            }
        }

        private void LoadLevel()
        {
            
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            //gameTime = new GameTime(gameTime.TotalGameTime / 5, gameTime.ElapsedGameTime / 5);
            // TODO: Add your update logic 
            Timer.Update((float)gameTime.ElapsedGameTime.TotalMilliseconds);
            CollisionEngine.Instance.Update(gameTime);
            LayerManager.Instance.UpdateAll(gameTime);
            Camera.update(gameTime);
            Camera.postUpdate(gameTime);
            lineToDraw.Clear();
            Bresenham.GetLine(hero.Transform.Position, e.Transform.Position, lineToDraw);
            base.Update(gameTime);
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
            spriteBatch.DrawString(font, "Triggering: " + bt.IsInsideTrigger(hero), new Vector2(1, 30), Color.Red);
            foreach (Vector2 point in lineToDraw)
            {
                spriteBatch.Draw(TextureUtil.CreateRectangle(1, Color.Black), point, Color.White);
            }
            spriteBatch.End();


            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
