using GameEngine2D.Engine.src.Layer;
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

        private float bdx = 0f;
        private float bdy = 0f;

        private KeyboardState currentKeyboardState;
        private KeyboardState previousKeyboardState;

        private bool canJump = true;
        private bool doubleJump = false;
        private float elapsedTime;
        private float steps;
        private float step;
        private float steps2;
        private float step2;
        private float t;
        public bool HasGravity { get; set; }  = Config.GRAVITY_ON;

        public float JumpStart { get; set; }

        protected FaceDirection CurrentFaceDirection { get; set; } = FaceDirection.RIGHT;

        public ControllableEntity(AbstractLayer layer, Entity parent, Vector2 startPosition, SpriteFont font = null) : base(layer, parent, startPosition, font)
        {
            ResetPosition(startPosition);
        }

        override public void Draw(GameTime gameTime)
        {

            currentKeyboardState = Keyboard.GetState();

            if (currentKeyboardState.IsKeyDown(Keys.R))
            {
                ResetPosition(new Vector2(9, 9) * Config.GRID);
            }

            elapsedTime = TimeUtil.GetElapsedTime(gameTime);

            if (!HasGravity && currentKeyboardState.IsKeyDown(Keys.Up))
            {
                Direction.Y -= Config.CHARACTER_SPEED * elapsedTime;
                if (!HasGravity)
                {
                    CurrentFaceDirection = FaceDirection.UP;
                }
            }

            if (HasGravity && currentKeyboardState.IsKeyDown(Keys.Space) && previousKeyboardState.IsKeyUp(Keys.Space) && (canJump || doubleJump))
            {
                if (canJump)
                {
                    doubleJump = true;
                } else
                {
                    doubleJump = false;
                }
                canJump = false;
                Direction.Y -= Config.JUMP_FORCE;
                JumpStart = (float)gameTime.TotalGameTime.TotalSeconds;
            }
            previousKeyboardState = currentKeyboardState;

            if (currentKeyboardState.IsKeyDown(Keys.Down))
            {
                Direction.Y += Config.CHARACTER_SPEED * elapsedTime;
                if (!HasGravity)
                {
                    CurrentFaceDirection = FaceDirection.DOWN;
                }
            }

            if (currentKeyboardState.IsKeyDown(Keys.Left))
            {
                Direction.X -= Config.CHARACTER_SPEED * elapsedTime;
                CurrentFaceDirection = FaceDirection.LEFT;
            }

            if (currentKeyboardState.IsKeyDown(Keys.Right))
            {
                Direction.X += Config.CHARACTER_SPEED * elapsedTime;
                CurrentFaceDirection = FaceDirection.RIGHT;
            }

            MouseState mouseState = Mouse.GetState();
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                GridCoordinates = CalculateGridCoord(new Vector2(mouseState.X, mouseState.Y));
                if (HasCollision && (!Scene.Instance.HasColliderAt(GridUtil.GetRightGrid(GridCoordinates)) &&
                    !Scene.Instance.HasColliderAt(GridUtil.GetLeftGrid(GridCoordinates)) &&
                    !Scene.Instance.HasColliderAt(GridUtil.GetUpperGrid(GridCoordinates)) &&
                    !Scene.Instance.HasColliderAt(GridUtil.GetBelowGrid(GridCoordinates))))
                {
                    Position = new Vector2(mouseState.X, mouseState.Y);
                }
                    
            }

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
            elapsedTime = TimeUtil.GetElapsedTime(gameTime);

            steps = (float)Math.Ceiling(Math.Abs((Direction.X + bdx) * elapsedTime));
            step = (float)(Direction.X + bdx) * elapsedTime / steps;
            while (steps > 0)
            {
                InCellLocation.X += step;

                if (HasCollision && InCellLocation.X >= Config.SPRITE_COLLISION_OFFSET && Scene.Instance.HasColliderAt(GridUtil.GetRightGrid(GridCoordinates)))
                {
                    InCellLocation.X = Config.SPRITE_COLLISION_OFFSET;
                }

                if (HasCollision && InCellLocation.X <= Config.SPRITE_COLLISION_OFFSET && Scene.Instance.HasColliderAt(GridUtil.GetLeftGrid(GridCoordinates)))
                {
                    InCellLocation.X = Config.SPRITE_COLLISION_OFFSET;
                }

                while (InCellLocation.X > 1) { InCellLocation.X--; GridCoordinates.X++; }
                while (InCellLocation.X < 0) { InCellLocation.X++; GridCoordinates.X--; }
                steps--;
            }
            Direction.X *= (float)Math.Pow(Config.FRICTION, elapsedTime);
            bdx *= (float)Math.Pow(Config.BUMB_FRICTION, elapsedTime);
            if (Math.Abs(Direction.X) <= 0.0005 * elapsedTime) Direction.X = 0;
            if (Math.Abs(bdx) <= 0.0005 * elapsedTime) bdx = 0;

            // Y
            if (HasGravity && !OnGround())
            {   
                if (JumpStart == 0)
                {
                    JumpStart = (float)gameTime.TotalGameTime.TotalSeconds;
                }
                t = (float)(gameTime.TotalGameTime.TotalSeconds - JumpStart) * Config.GRAVITY_T_MULTIPLIER;
                Direction.Y += GetGravityConstant() * t/* * elapsedTime*/;
                canJump = false;
            }
                
            if (HasGravity && OnGround() /*|| direction.Y < 0*/)
            {
                //fallStartY = footY;
                canJump = true;
                doubleJump = true;
                JumpStart = 0;
            }

            steps2 = (float)Math.Ceiling(Math.Abs((Direction.Y + bdy) * elapsedTime));
            step2 = (float)(Direction.Y + bdy) * elapsedTime / steps2;
            while (steps2 > 0)
            {
                InCellLocation.Y += step2;

                if (HasCollision && InCellLocation.Y > Config.SPRITE_COLLISION_OFFSET && Scene.Instance.HasColliderAt(GridUtil.GetBelowGrid(GridCoordinates)))
                {
                    Direction.Y = 0;
                    InCellLocation.Y = Config.SPRITE_COLLISION_OFFSET;
                    bdy = 0;
                }

                if (HasCollision && InCellLocation.Y < Config.SPRITE_COLLISION_OFFSET && Scene.Instance.HasColliderAt(GridUtil.GetUpperGrid(GridCoordinates)))
                {
                    InCellLocation.Y = Config.SPRITE_COLLISION_OFFSET;
                }
                   
                while (InCellLocation.Y > 1) { InCellLocation.Y--; GridCoordinates.Y++; }
                while (InCellLocation.Y < 0) { InCellLocation.Y++; GridCoordinates.Y--; }
                steps2--;
            }
            Direction.Y *= (float)Math.Pow(Config.FRICTION, elapsedTime);
            bdy *= (float)Math.Pow(Config.BUMB_FRICTION, elapsedTime);
            if (Math.Abs(Direction.Y) <= 0.0005 * elapsedTime) Direction.Y = 0;
            if (Math.Abs(bdy) <= 0.0005 * elapsedTime) bdy = 0;


            //currentPosition = new Vector2((float)(gridCoord.X + inCellLocation.X) * Constants.GRID, (float)(gridCoord.Y + inCellLocation.Y) * Constants.GRID);
            //currentPosition = (gridCoord + inCellLocation) * Constants.GRID;

            //System.Diagnostics.Debug.WriteLine(position);
            //position = new Vector2((float)(cx + xr), (float)(cy + yr));

            base.Update(gameTime);
        }

        public override void PostUpdate(GameTime gameTime)
        {
            //currentPosition.X = (int)((gridCoord.X + inCellLocation.X) * Constants.GRID);
            //currentPosition.Y = (int)((gridCoord.Y + inCellLocation.Y) * Constants.GRID);
            Position = (GridCoordinates + InCellLocation) * Config.GRID;
            base.PostUpdate(gameTime);
        }

        /*public void SetPositionPixel(Vector2 posiiton)
        {
            gridCoord = new Vector2((int)(posiiton.X / Constants.GRID), (int)(posiiton.Y / Constants.GRID));
            inCellLocation = new Vector2((posiiton.X - gridCoord.X * Constants.GRID) / Constants.GRID, (posiiton.Y - gridCoord.Y * Constants.GRID) / Constants.GRID);
        }*/

        public float GetGravityConstant()
        {
            return Config.GRAVITY_FORCE;
        }

        private bool OnGround()
        {
            bool onGround = Scene.Instance.HasColliderAt(GridUtil.GetBelowGrid(GridCoordinates)) /*&& inCellLocation.Y == 1 && direction.Y >= 0*/;
            return onGround;
        }

        public void ResetPosition(Vector2 position)
        {
            InCellLocation = Vector2.Zero;
            this.Position = StartPosition = position;
            this.JumpStart = 0;
        }

        protected enum FaceDirection
        {
            UP,
            DOWN,
            LEFT,
            RIGHT
        }
    }
}
