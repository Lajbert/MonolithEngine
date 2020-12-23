﻿using _2DGameEngine.Entities;
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
    class ControllableEntity : Entity, Updatable
    {

        //private float dx = 0;
        //private float dy = 0;
        private Vector2 direction = Vector2.Zero;

        private float bdx = 0f;
        private float bdy = 0f;

        private MyLevel level;

        public ControllableEntity(MyLevel level, HasChildren parent, GraphicsDevice graphicsDevice, Texture2D texture2D, Vector2 startPosition, SpriteFont font = null) : base(parent, graphicsDevice, texture2D, startPosition, font)
        {
            this.level = level;
            //gridCoord = new Vector2((int)Math.Floor(startPosition.X / Constants.GRID), (int)Math.Floor(startPosition.Y / Constants.GRID));
            //SetPosition(new Vector2((int)startPosition.X, (int)startPosition.Y));
        }

        override public void Draw(GameTime gameTime)
        {
            KeyboardState kstate = Keyboard.GetState();


            //moveX = moveY = 0;

            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (kstate.IsKeyDown(Keys.Up))
            //if (kstate.IsKeyDown(Keys.Up) && (HasCollision() && !level.HasColliderAt(GridUtil.GetUpperGrid(gridCoord))))
            {
                //moveY = -1;
                //dy -= Constants.CHARACTER_SPEED * elapsedTime;
                direction.Y -= Constants.CHARACTER_SPEED * elapsedTime;
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

            float steps = (float)Math.Ceiling(Math.Abs((direction.X + bdx) * elapsedTime));
            float step = (float)(direction.X + bdx) * elapsedTime / steps;
            while (steps > 0)
            {
                inCellLocation.X += step;

                if (HasCollision() && inCellLocation.X >= 0 && level.HasColliderAt(GridUtil.GetRightGrid(gridCoord)))
                {
                    inCellLocation.X = 0f;
                    Logger.Log("GRID COORD: " + gridCoord);
                }

                if (HasCollision() && inCellLocation.X <= 0 && level.HasColliderAt(GridUtil.GetLeftGrid(gridCoord)))
                {
                    inCellLocation.X = 0f;
                    Logger.Log("GRID COORD: " + gridCoord);
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
            float steps2 = (float)Math.Ceiling(Math.Abs((direction.Y + bdy) * elapsedTime));
            float step2 = (float)(direction.Y + bdy) * elapsedTime / steps2;
            while (steps2 > 0)
            {
                inCellLocation.Y += step2;

                if (HasCollision() && inCellLocation.Y > 0 && level.HasColliderAt(GridUtil.GetBelowGrid(gridCoord)))
                {
                   // direction.Y = 0;
                    inCellLocation.Y = 0;
                    bdy = 0;
                    Logger.Log("GRID COORD: " + gridCoord);
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
    }
}