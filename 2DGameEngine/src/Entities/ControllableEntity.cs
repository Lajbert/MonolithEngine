using _2DGameEngine.Entities;
using _2DGameEngine.Entities.Interfaces;
using _2DGameEngine.Global;
using _2DGameEngine.Level;
using _2DGameEngine.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace _2DGameEngine
{
    class ControllableEntity : Entity, Updatable, GravityApplicable
    {

        //private float dx = 0;
        //private float dy = 0;
        private Vector2 direction = Vector2.Zero;

        //between 0 and 1: where the object is inside the grid cell
        //private float xr = 0.5f;
        //private float yr = 1.0f;
        protected Vector2 inCellLocation = new Vector2(0.5f, 1.0f);

        //grid coordinates
        //private float cx = 0f;
        //private float cy = 0f;
        protected Vector2 gridCoord = Vector2.Zero;

        private float bdx = 0f;
        private float bdy = 0f;

        private bool canJump = true;

        public ControllableEntity(MyLevel level, HasChildren parent, GraphicsDevice graphicsDevice, Texture2D texture2D, Vector2 startPosition, SpriteFont font = null) : base(level, parent, graphicsDevice, texture2D, startPosition, font)
        {
            SetPosition(startPosition);
        }

        override public void Draw(GameTime gameTime)
        {
            KeyboardState kstate = Keyboard.GetState();

            if (kstate.IsKeyDown(Keys.R))
            //if (kstate.IsKeyDown(Keys.Up) && (HasCollision() && !level.HasColliderAt(GridUtil.GetUpperGrid(gridCoord))))
            {
                SetPosition(new Vector2(9 * Constants.GRID, 9 * Constants.GRID));
            }


            //moveX = moveY = 0;

            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (kstate.IsKeyDown(Keys.Up))
            //if (kstate.IsKeyDown(Keys.Up) && (HasCollision() && !level.HasColliderAt(GridUtil.GetUpperGrid(gridCoord))))
            {
                //moveY = -1;
                //dy -= Constants.CHARACTER_SPEED * elapsedTime;
                direction.Y -= Constants.CHARACTER_SPEED * elapsedTime;
            }

            if (kstate.IsKeyDown(Keys.Space) && canJump)
            //if (kstate.IsKeyDown(Keys.Up) && (HasCollision() && !level.HasColliderAt(GridUtil.GetUpperGrid(gridCoord))))
            {
                //moveY = -1;
                //dy -= Constants.CHARACTER_SPEED * elapsedTime;
                canJump = false;
                direction.Y -= Constants.JUMP_FORCE * elapsedTime;
            }


            if (kstate.IsKeyDown(Keys.Down))
            //if (kstate.IsKeyDown(Keys.Down) && (HasCollision() && !level.HasColliderAt(GridUtil.GetBelowGrid(gridCoord))))
            {
                //moveY = 1;
                //dy += Constants.CHARACTER_SPEED * elapsedTime;
                direction.Y += Constants.CHARACTER_SPEED * elapsedTime;
            }

            if (kstate.IsKeyDown(Keys.Left))
            //if (kstate.IsKeyDown(Keys.Left) && (HasCollision() && !level.HasColliderAt(GridUtil.GetLeftGrid(gridCoord))))
            {
                //moveX = -1;
                //dx -= Constants.CHARACTER_SPEED * elapsedTime;
                direction.X -= Constants.CHARACTER_SPEED * elapsedTime;
            }

            if (kstate.IsKeyDown(Keys.Right))
            //if (kstate.IsKeyDown(Keys.Right) && (HasCollision() && !level.HasColliderAt(GridUtil.GetRightGrid(gridCoord))))
            {
                //moveX = 1;
                //dx += Constants.CHARACTER_SPEED * elapsedTime;
                direction.X += Constants.CHARACTER_SPEED * elapsedTime;
            }

            MouseState mouseState = Mouse.GetState();
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                //gridCoord = new Vector2((int)Math.Floor((double)mouseState.X / Constants.GRID), (int)Math.Floor((double)mouseState.Y / Constants.GRID));
                gridCoord = GetGridCoord(new Vector2(mouseState.X, mouseState.Y));
                if (HasCollision() && (!level.HasColliderAt(GridUtil.GetRightGrid(gridCoord)) &&
                    !level.HasColliderAt(GridUtil.GetLeftGrid(gridCoord)) &&
                    !level.HasColliderAt(GridUtil.GetUpperGrid(gridCoord)) &&
                    !level.HasColliderAt(GridUtil.GetBelowGrid(gridCoord))))
                {
                    position = new Vector2(mouseState.X, mouseState.Y);
                }
                    
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
            spriteBatch.Begin();
            spriteBatch.DrawString(font, inCellLocation.X + " : " + inCellLocation.Y, new Vector2(10,10), Color.White);
            spriteBatch.End();
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

            float steps = (float)Math.Ceiling(Math.Abs((direction.X + bdx) * elapsedTime));
            float step = (float)(direction.X + bdx) * elapsedTime / steps;
            while (steps > 0)
            {
                inCellLocation.X += step;

                if (HasCollision() && inCellLocation.X >= 0 && level.HasColliderAt(GridUtil.GetRightGrid(gridCoord)))
                {
                    inCellLocation.X = 0f;
                }

                if (HasCollision() && inCellLocation.X <= 0 && level.HasColliderAt(GridUtil.GetLeftGrid(gridCoord)))
                {
                    inCellLocation.X = 0f;
                }

                while (inCellLocation.X > 1) { inCellLocation.X--; gridCoord.X++; }
                while (inCellLocation.X < 0) { inCellLocation.X++; gridCoord.X--; }
                steps--;
            }
            direction.X *= (float)Math.Pow(Constants.FRICTION, elapsedTime);
            bdx *= (float)Math.Pow(Constants.BUMB_FRICTION, elapsedTime);
            if (Math.Abs(direction.X) <= 0.0005 * elapsedTime) direction.X = 0;
            if (Math.Abs(bdx) <= 0.0005 * elapsedTime) bdx = 0;

            // Y
            if (!OnGround())
            {
                direction.Y += GetGravity() * elapsedTime;
                canJump = false;
            }
                
            if (OnGround() /*|| direction.Y < 0*/)
            {
                //fallStartY = footY;
                canJump = true;
            }

            float steps2 = (float)Math.Ceiling(Math.Abs((direction.Y + bdy) * elapsedTime));
            float step2 = (float)(direction.Y + bdy) * elapsedTime / steps2;
            while (steps2 > 0)
            {
                inCellLocation.Y += step2;

                if (HasCollision() && inCellLocation.Y > 0 && level.HasColliderAt(GridUtil.GetBelowGrid(gridCoord)))
                {
                    direction.Y = 0;
                    inCellLocation.Y = 0;
                    bdy = 0;
                }

                if (HasCollision() && inCellLocation.Y < 0 && level.HasColliderAt(GridUtil.GetUpperGrid(gridCoord)))
                {
                    inCellLocation.Y = 0f;
                }
                   
                while (inCellLocation.Y > 1) { inCellLocation.Y--; gridCoord.Y++; }
                while (inCellLocation.Y < 0) { inCellLocation.Y++; gridCoord.Y--; }
                steps2--;
            }
            direction.Y *= (float)Math.Pow(Constants.FRICTION, elapsedTime);
            bdy *= (float)Math.Pow(Constants.BUMB_FRICTION, elapsedTime);
            if (Math.Abs(direction.Y) <= 0.0005 * elapsedTime) direction.Y = 0;
            if (Math.Abs(bdy) <= 0.0005 * elapsedTime) bdy = 0;


            position = new Vector2((float)(gridCoord.X + inCellLocation.X) * Constants.GRID, (float)(gridCoord.Y + inCellLocation.Y) * Constants.GRID);


            //System.Diagnostics.Debug.WriteLine(position);
            //position = new Vector2((float)(cx + xr), (float)(cy + yr));
        }

        /*public void SetPositionPixel(Vector2 posiiton)
        {
            gridCoord = new Vector2((int)(posiiton.X / Constants.GRID), (int)(posiiton.Y / Constants.GRID));
            inCellLocation = new Vector2((posiiton.X - gridCoord.X * Constants.GRID) / Constants.GRID, (posiiton.Y - gridCoord.Y * Constants.GRID) / Constants.GRID);
        }*/

        public bool HasGravity()
        {
            return Constants.GRAVITY_ON;
        }

        public float GetGravity()
        {
            return Constants.GRAVITY_FORCE;
        }

        private bool OnGround()
        {
            bool onGround = level.HasColliderAt(GridUtil.GetBelowGrid(gridCoord)) /*&& inCellLocation.Y == 1 && direction.Y >= 0*/;
            return onGround;
        }

        public void SetPosition(Vector2 positon)
        {
            gridCoord = GetGridCoord(position);
            inCellLocation = new Vector2(0f, 0f);
        }

    }
}
