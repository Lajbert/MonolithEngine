using GameEngine2D.Entities;
using GameEngine2D.Entities.Interfaces;
using GameEngine2D.Global;
using GameEngine2D.src;
using GameEngine2D.src.Layer;
using GameEngine2D.src.Util;
using GameEngine2D.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace GameEngine2D
{
    class ControllableEntity : Entity, IGravityApplicable
    {

        //private float dx = 0;
        //private float dy = 0;
        private Vector2 direction = Vector2.Zero;

        private float bdx = 0f;
        private float bdy = 0f;

        private KeyboardState currentKeyboardState;
        private KeyboardState previousKeyboardState;

        private bool canJump = true;
        private bool doubleJump = false;
        private bool gravity = Constants.GRAVITY_ON;

        private float jumpStart;

        private MoveState currentState = MoveState.IDLE;
        private MoveState previousState = MoveState.IDLE;

        private FaceDirection currentFaceDirection = FaceDirection.RIGHT;
        private FaceDirection prevFaceDirection = FaceDirection.RIGHT;

        public ControllableEntity(GraphicsLayer layer, Entity parent, GraphicsDevice graphicsDevice, Vector2 startPosition, SpriteFont font = null) : base(layer, parent, graphicsDevice, startPosition, font)
        {
            SetPosition(startPosition);
        }

        override public void Draw(GameTime gameTime)
        {
            previousState = currentState;
            prevFaceDirection = currentFaceDirection;
            if (!canJump)
            {
                currentState = MoveState.JUMPING;
            }
            else
            {
                currentState = MoveState.IDLE;
            }

            currentKeyboardState = Keyboard.GetState();

            if (currentKeyboardState.IsKeyDown(Keys.R))
            {
                SetPosition(new Vector2(9, 9) * Constants.GRID);
            }


            //moveX = moveY = 0;

            float elapsedTime = TimeUtil.GetElapsedTime(gameTime);

            if (!HasGravity() && currentKeyboardState.IsKeyDown(Keys.Up))
            {
                //moveY = -1;
                //dy -= Constants.CHARACTER_SPEED * elapsedTime;
                direction.Y -= Constants.CHARACTER_SPEED * elapsedTime;
                if (!HasGravity())
                {
                    currentFaceDirection = FaceDirection.UP;
                }
            }

            if (HasGravity() && currentKeyboardState.IsKeyDown(Keys.Space) && previousKeyboardState.IsKeyUp(Keys.Space) && (canJump || doubleJump))
            {
                //moveY = -1;
                //dy -= Constants.CHARACTER_SPEED * elapsedTime;
                if (canJump)
                {
                    doubleJump = true;
                } else
                {
                    doubleJump = false;
                }
                canJump = false;
                direction.Y -= Constants.JUMP_FORCE;
                jumpStart = (float)gameTime.TotalGameTime.TotalSeconds;
                currentState = MoveState.JUMPING;
            }
            previousKeyboardState = currentKeyboardState;

            if (currentKeyboardState.IsKeyDown(Keys.Down))
            {
                //moveY = 1;
                //dy += Constants.CHARACTER_SPEED * elapsedTime;
                direction.Y += Constants.CHARACTER_SPEED * elapsedTime;
                if (!HasGravity())
                {
                    currentFaceDirection = FaceDirection.DOWN;
                }
            }

            if (currentKeyboardState.IsKeyDown(Keys.Left))
            {
                //moveX = -1;
                //dx -= Constants.CHARACTER_SPEED * elapsedTime;
                direction.X -= Constants.CHARACTER_SPEED * elapsedTime;
                if (currentState != MoveState.JUMPING)
                {
                    currentState = MoveState.MOVING_LEFT;
                }
                
                currentFaceDirection = FaceDirection.LEFT;
            }

            if (currentKeyboardState.IsKeyDown(Keys.Right))
            {
                //moveX = 1;
                //dx += Constants.CHARACTER_SPEED * elapsedTime;
                direction.X += Constants.CHARACTER_SPEED * elapsedTime;
                //currentAnimation = moveRightAnimation;
                if (currentState != MoveState.JUMPING) 
                {
                    currentState = MoveState.MOVING_RIGHT;
                }
                currentFaceDirection = FaceDirection.RIGHT;
            }

            MouseState mouseState = Mouse.GetState();
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                gridCoord = CalculateGridCoord(new Vector2(mouseState.X, mouseState.Y));
                if (HasCollision() && (!Scene.Instance.HasColliderAt(GridUtil.GetRightGrid(gridCoord)) &&
                    !Scene.Instance.HasColliderAt(GridUtil.GetLeftGrid(gridCoord)) &&
                    !Scene.Instance.HasColliderAt(GridUtil.GetUpperGrid(gridCoord)) &&
                    !Scene.Instance.HasColliderAt(GridUtil.GetBelowGrid(gridCoord))))
                {
                    currentPosition = new Vector2(mouseState.X, mouseState.Y);
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

#if GRAPHICS_DEBUG
            spriteBatch.Begin();
            spriteBatch.DrawString(font, inCellLocation.X + " : " + inCellLocation.Y, new Vector2(10,10), Color.White);
            spriteBatch.End();
#endif

            base.Draw(gameTime);
        }

        public override void PreUpdate(GameTime gameTime)
        {
            base.PreUpdate(gameTime);
        }
        public override void Update(GameTime gameTime)
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

            SetAnimation();


            float elapsedTime = TimeUtil.GetElapsedTime(gameTime);

            float steps = (float)Math.Ceiling(Math.Abs((direction.X + bdx) * elapsedTime));
            float step = (float)(direction.X + bdx) * elapsedTime / steps;
            while (steps > 0)
            {
                inCellLocation.X += step;

                if (HasCollision() && inCellLocation.X >= Constants.SPRITE_COLLISION_OFFSET && Scene.Instance.HasColliderAt(GridUtil.GetRightGrid(gridCoord)))
                {
                    inCellLocation.X = Constants.SPRITE_COLLISION_OFFSET;
                }

                if (HasCollision() && inCellLocation.X <= Constants.SPRITE_COLLISION_OFFSET && Scene.Instance.HasColliderAt(GridUtil.GetLeftGrid(gridCoord)))
                {
                    inCellLocation.X = Constants.SPRITE_COLLISION_OFFSET;
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
            if (HasGravity() && !OnGround())
            {   
                if (jumpStart == 0)
                {
                    jumpStart = (float)gameTime.TotalGameTime.TotalSeconds;
                }
                float t = (float)(gameTime.TotalGameTime.TotalSeconds - jumpStart) * Constants.GRAVITY_T_MULTIPLIER;
                direction.Y += GetGravityConstant() * t * elapsedTime;
                //direction.Y += GetGravityConstant() * (float)Math.Pow(t, 2) * elapsedTime;
                canJump = false;
            }
                
            if (HasGravity() && OnGround() /*|| direction.Y < 0*/)
            {
                //fallStartY = footY;
                canJump = true;
                doubleJump = true;
                jumpStart = 0;
            }

            float steps2 = (float)Math.Ceiling(Math.Abs((direction.Y + bdy) * elapsedTime));
            float step2 = (float)(direction.Y + bdy) * elapsedTime / steps2;
            while (steps2 > 0)
            {
                inCellLocation.Y += step2;

                if (HasCollision() && inCellLocation.Y > Constants.SPRITE_COLLISION_OFFSET && Scene.Instance.HasColliderAt(GridUtil.GetBelowGrid(gridCoord)))
                {
                    direction.Y = 0;
                    inCellLocation.Y = Constants.SPRITE_COLLISION_OFFSET;
                    bdy = 0;
                }

                if (HasCollision() && inCellLocation.Y < Constants.SPRITE_COLLISION_OFFSET && Scene.Instance.HasColliderAt(GridUtil.GetUpperGrid(gridCoord)))
                {
                    inCellLocation.Y = Constants.SPRITE_COLLISION_OFFSET;
                }
                   
                while (inCellLocation.Y > 1) { inCellLocation.Y--; gridCoord.Y++; }
                while (inCellLocation.Y < 0) { inCellLocation.Y++; gridCoord.Y--; }
                steps2--;
            }
            direction.Y *= (float)Math.Pow(Constants.FRICTION, elapsedTime);
            bdy *= (float)Math.Pow(Constants.BUMB_FRICTION, elapsedTime);
            if (Math.Abs(direction.Y) <= 0.0005 * elapsedTime) direction.Y = 0;
            if (Math.Abs(bdy) <= 0.0005 * elapsedTime) bdy = 0;


            //currentPosition = new Vector2((float)(gridCoord.X + inCellLocation.X) * Constants.GRID, (float)(gridCoord.Y + inCellLocation.Y) * Constants.GRID);
            currentPosition = (gridCoord + inCellLocation) * Constants.GRID;

            //System.Diagnostics.Debug.WriteLine(position);
            //position = new Vector2((float)(cx + xr), (float)(cy + yr));

            base.Update(gameTime);
        }

        private void SetAnimation()
        {
            if (HasGravity() && currentState == MoveState.JUMPING)
            {
                if (previousState != MoveState.JUMPING)
                {
                    if (currentFaceDirection == FaceDirection.LEFT)
                    {
                        this.currentAnimation = jumpLeftAnimation;
                    } else
                    {
                        this.currentAnimation = jumpRightAnimation;
                    }
                    
                } else if (prevFaceDirection != currentFaceDirection)
                {
                    if (currentFaceDirection == FaceDirection.LEFT)
                    {
                        this.currentAnimation = jumpLeftAnimation;
                    }
                    else
                    {
                        this.currentAnimation = jumpRightAnimation;
                    }
                }
                return;
            }
            if (currentState == MoveState.IDLE)
            {
                if (previousState != MoveState.IDLE)
                {
                    if (currentFaceDirection == FaceDirection.RIGHT)
                    {
                        this.currentAnimation = idleAnimationRight;
                    } 
                    else if (currentFaceDirection == FaceDirection.LEFT)
                    {
                        this.currentAnimation = idleAnimationLeft;
                    }
                    else if (!HasGravity() && currentFaceDirection == FaceDirection.UP)
                    {
                        this.currentAnimation = idleAnimationLeft;
                    }
                    else if (!HasGravity() && currentFaceDirection == FaceDirection.DOWN)
                    {
                        this.currentAnimation = idleAnimationLeft;
                    }

                }
            }
            else if (currentState == MoveState.MOVING_RIGHT)
            {
                if (previousState != MoveState.MOVING_RIGHT)
                {
                    this.currentAnimation = moveRightAnimation;
                }
            }
            else if (currentState == MoveState.MOVING_LEFT)
            {
                if (previousState != MoveState.MOVING_LEFT)
                {
                    this.currentAnimation = moveLeftAnimation;
                }
            }
        }

        public override void PostUpdate(GameTime gameTime)
        {
            //currentPosition.X = (int)((gridCoord.X + inCellLocation.X) * Constants.GRID);
            //currentPosition.Y = (int)((gridCoord.Y + inCellLocation.Y) * Constants.GRID);
            base.PostUpdate(gameTime);
        }

        /*public void SetPositionPixel(Vector2 posiiton)
        {
            gridCoord = new Vector2((int)(posiiton.X / Constants.GRID), (int)(posiiton.Y / Constants.GRID));
            inCellLocation = new Vector2((posiiton.X - gridCoord.X * Constants.GRID) / Constants.GRID, (posiiton.Y - gridCoord.Y * Constants.GRID) / Constants.GRID);
        }*/

        public bool HasGravity()
        {
            return gravity;
        }

        public void SetGravity(bool gravityOn)
        {
            gravity = gravityOn;
        }

        public float GetGravityConstant()
        {
            return Constants.GRAVITY_FORCE;
        }

        private bool OnGround()
        {
            bool onGround = Scene.Instance.HasColliderAt(GridUtil.GetBelowGrid(gridCoord)) /*&& inCellLocation.Y == 1 && direction.Y >= 0*/;
            return onGround;
        }

        public void SetPosition(Vector2 position)
        {
            inCellLocation = Vector2.Zero;
            this.currentPosition = startPosition = position;
            this.jumpStart = 0;
        }

        private enum MoveState
        {
            IDLE,
            MOVING_LEFT,
            MOVING_RIGHT,
            MOVING_UP,
            MOVING_DOWN,
            FALLING,
            JUMPING
        }

        private enum FaceDirection
        {
            UP,
            DOWN,
            LEFT,
            RIGHT
        }
    }
}
