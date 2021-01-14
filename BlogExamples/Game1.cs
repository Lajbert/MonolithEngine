using GameEngine2D.Engine.Source.Graphics.Primitives;
using GameEngine2D.Engine.Source.Util;
using GameEngine2D.Entities;
using GameEngine2D.Source.Camera2D;
using GameEngine2D.Source.Layer;
using GameEngine2D.Source.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BlogExamples
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Camera camera;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            SpriteUtil.GraphicsDeviceManager = _graphics;
            SpriteUtil.Content = Content;

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            _spriteBatch.Draw(SpriteUtil.CreateRectangle(50, Color.Black), new Vector2(100, 100), Color.White);
            _spriteBatch.Draw(SpriteUtil.CreateCircle(50, Color.Red), new Vector2(200, 100), Color.White);
            //Line l = new Line(null, new Vector2(250, 100), new Vector2(350, 100), Color.White, 1);

            Vector2 From = new Vector2(270, 125);
            Vector2 To = new Vector2(550, 125);
             float thickness = 1f;
            Color color = Color.White;
            Texture2D Sprite = SpriteUtil.CreateRectangle(1, Color.White);
            float length = Vector2.Distance(From, To);
            float angleRad = MathUtil.AngleFromVectors(From, To);
            Vector2 Origin = new Vector2(0f, 0f);
            Vector2 Scale = new Vector2(length, thickness);
            _spriteBatch.Draw(Sprite, From, null, color, angleRad, Origin, Scale, SpriteEffects.None, 0);
            _spriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
