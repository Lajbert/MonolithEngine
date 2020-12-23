using _2DGameEngine.Entities;
using _2DGameEngine.Entities.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace _2DGameEngine
{
    class ControllableEntity : StaticEntity, Updatable
    {
        private float speed;
        private int moveX = 0;
        private int moveY = 0;

        private double dx = 0;
        private double dy = 0;

        //between 0 and 1: where the object is inside the grid cell
        private double xr = 0.5f;
        private double yr = 1.0f;

        //grid coordinates
        private float cx = 0f;
        private float cy = 0f;

        private float bumpFriction = 0.99f;
        private float bdx = 0f;
        private float bdy = 0f;

        private float friction = 0.99f;

        private static double SPEED = 0.01f;
        private static int GRID = 2;

        public ControllableEntity(HasChildren parent, GraphicsDevice graphicsDevice, Texture2D texture2D, Vector2 startPosition, float speed = 0.5f) : base(parent, graphicsDevice, texture2D, startPosition)
        {
            this.speed = speed;
        }

        override public void Draw(GameTime gameTime)
        {
            KeyboardState kstate = Keyboard.GetState();


            //moveX = moveY = 0;

            double elapsedTime = gameTime.ElapsedGameTime.TotalMilliseconds;

            if (kstate.IsKeyDown(Keys.Up)) 
            {
                //moveY = -1;
                dy -= SPEED * elapsedTime;
            }


            if (kstate.IsKeyDown(Keys.Down))
            {
                //moveY = 1;
                dy += SPEED * elapsedTime;
            }


            if (kstate.IsKeyDown(Keys.Left))
            {
                //moveX = -1;
                dx -= SPEED * elapsedTime;
            }

            if (kstate.IsKeyDown(Keys.Right))
            {
                //moveX = 1;
                dx += SPEED * elapsedTime;
            }

            /*if (heroPosition.X > _graphics.PreferredBackBufferWidth - ballTexture.Width / 2)
                ballPosition.X = _graphics.PreferredBackBufferWidth - ballTexture.Width / 2;
            else if (ballPosition.X < ballTexture.Width / 2)
                ballPosition.X = ballTexture.Width / 2;

            if (ballPosition.Y > _graphics.PreferredBackBufferHeight - ballTexture.Height / 2)
                ballPosition.Y = _graphics.PreferredBackBufferHeight - ballTexture.Height / 2;
            else if (ballPosition.Y < ballTexture.Height / 2)
                ballPosition.Y = ballTexture.Height / 2;*/


            /*
            Vector2 move = new Vector2(moveX, moveY);
            if (move != Vector2.Zero)
            {
                move.Normalize();
            }

            //System.Diagnostics.Debug.WriteLine(move);
            position += move * speed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            */
            base.Draw(gameTime);
        }

        public void Update(GameTime gameTime)
        {
            /*
            Vector2 move = new Vector2(moveX, moveY);
            if (move != Vector2.Zero) 
            {
                move.Normalize();
            }
            
            //System.Diagnostics.Debug.WriteLine(move);
            position += move * speed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            */

            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            float steps = (float)Math.Ceiling(Math.Abs((dx + bdx) * elapsedTime));
            float step = (float)(dx + bdx) * elapsedTime / steps;
            while (steps > 0)
            {
                xr += step;

                // [ add X collisions checks here ]

                while (xr > 1) { xr--; cx++; }
                while (xr < 0) { xr++; cx--; }
                steps--;
            }
            dx *= Math.Pow(friction, elapsedTime);
            bdx *= (float)Math.Pow(bumpFriction, elapsedTime);
            if (Math.Abs(dx) <= 0.0005 * elapsedTime) dx = 0;
            if (Math.Abs(bdx) <= 0.0005 * elapsedTime) bdx = 0;

            // Y
            double steps2 = Math.Ceiling(Math.Abs((dy + bdy) * elapsedTime));
            double step2 = (dy + bdy) * elapsedTime / steps2;
            while (steps2 > 0)
            {
                yr += step2;

                // [ add Y collisions checks here ]

                while (yr > 1) { yr--; cy++; }
                while (yr < 0) { yr++; cy--; }
                steps2--;
            }
            dy *= Math.Pow(friction, elapsedTime);
            bdy *= (float)Math.Pow(bumpFriction, elapsedTime);
            if (Math.Abs(dy) <= 0.0005 * elapsedTime) dy = 0;
            if (Math.Abs(bdy) <= 0.0005 * elapsedTime) bdy = 0;

            position = new Vector2((float)(cx + xr) * GRID, (float)(cy + yr) * GRID);
            //System.Diagnostics.Debug.WriteLine(position);
            //position = new Vector2((float)(cx + xr), (float)(cy + yr));
        }
    }
}
