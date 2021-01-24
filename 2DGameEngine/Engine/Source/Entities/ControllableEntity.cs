using GameEngine2D.Engine.Source.Entities;
using GameEngine2D.Engine.Source.Entities.Controller;
using GameEngine2D.Entities;
using GameEngine2D.Entities.Interfaces;
using GameEngine2D.Global;
using GameEngine2D.Source;
using GameEngine2D.Source.Layer;
using GameEngine2D.Source.Util;
using GameEngine2D.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace GameEngine2D
{
    public class ControllableEntity : Entity
    {

        private float bdx = 0f;
        private float bdy = 0f;

        protected UserInputController UserInput;

        protected bool canJump = true;
        protected bool canDoubleJump = false;
        protected float elapsedTime;
        private float steps;
        private float step;
        private float steps2;
        private float step2;
        private float t;

        protected float Friction = Config.FRICTION;
        protected float BumpFriction = Config.BUMP_FRICTION;

        protected float MovementSpeed = Config.CHARACTER_SPEED;

        protected Vector2 JumpModifier = Vector2.Zero;

        public float GravityValue = Config.GRAVITY_FORCE;

        public bool HasGravity { get; set; }  = Config.GRAVITY_ON;

        public float FallStartedAt { get; set; }

        protected GameTime GameTime;

        protected Direction CurrentFaceDirection { get; set; } = Engine.Source.Entities.Direction.RIGHT;

        public ControllableEntity(Layer layer, Entity parent, Vector2 startPosition, Texture2D texture = null, bool collider = false, SpriteFont font = null) : base(layer, parent, startPosition, texture, collider, font)
        {
            Active = true;
            ResetPosition(startPosition);
        }

        override public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
#if GRAPHICS_DEBUG
            spriteBatch.DrawString(font, InCellLocation.X + " : " + InCellLocation.Y, new Vector2(10,10), Color.White);
#endif

            base.Draw(spriteBatch, gameTime);
        }

        public override void PreUpdate(GameTime gameTime)
        {
            if (UserInput != null)
            {
                UserInput.Update();
            }
            base.PreUpdate(gameTime);
        }

        public override void Update(GameTime gameTime)
        {

            elapsedTime = TimeUtil.GetElapsedTime(gameTime);

            this.GameTime = gameTime;

            if (CollisionChecker.HasObjectAtWithTag(GridCoordinates, "Ladder") && !OnGround()) {
                if (HasGravity)
                {
                    Velocity.Y = 0;
                    MovementSpeed /= 2;
                    HasGravity = false;
                }
                FallStartedAt = 0;
            } else
            {
                if (!HasGravity)
                {
                    HasGravity = true;
                    MovementSpeed = Config.CHARACTER_SPEED;
                    if (Velocity.Y < -0.5) 
                    {
                        Velocity.Y -= Config.JUMP_FORCE / 2;
                    }
                    
                }
            }

            steps = (float)Math.Ceiling(Math.Abs((Velocity.X + bdx) * elapsedTime));
            step = (float)(Velocity.X + bdx) * elapsedTime / steps;
            while (steps > 0)
            {
                InCellLocation.X += step;

                if (HasCollision && InCellLocation.X >= CollisionOffsetLeft && CollisionChecker.HasBlockingColliderAt(GridUtil.GetRightGrid(GridCoordinates)))
                {
                    //Direction.X = 0;
                    //bdx = 0;
                    if (!CollisionChecker.GetColliderAt(GridUtil.GetRightGrid(GridCoordinates)).HasTag("Platform"))
                    {
                        InCellLocation.X = CollisionOffsetLeft;
                    }
                        
                }

                if (HasCollision && InCellLocation.X <= CollisionOffsetRight && CollisionChecker.HasBlockingColliderAt(GridUtil.GetLeftGrid(GridCoordinates)))
                {
                    //Direction.X = 0;
                    //bdx = 0;
                    if (!CollisionChecker.GetColliderAt(GridUtil.GetLeftGrid(GridCoordinates)).HasTag("Platform"))
                    {
                        InCellLocation.X = CollisionOffsetRight;
                    }
                }

                while (InCellLocation.X > 1) { InCellLocation.X--; GridCoordinates.X++; }
                while (InCellLocation.X < 0) { InCellLocation.X++; GridCoordinates.X--; }
                steps--;
            }
            Velocity.X *= (float)Math.Pow(Friction, elapsedTime);
            bdx *= (float)Math.Pow(BumpFriction, elapsedTime);

            //rounding stuff
            if (Math.Abs(Velocity.X) <= 0.0005 * elapsedTime) Velocity.X = 0;
            if (Math.Abs(bdx) <= 0.0005 * elapsedTime) bdx = 0;

            // Y
            if (HasCollision && HasGravity && !OnGround())
            {

                GravityValue = Config.GRAVITY_FORCE;
                JumpModifier = Vector2.Zero;

                if (CollisionChecker.HasObjectAtWithTag(GridUtil.GetRightGrid(GridCoordinates), "SlideWall")
                    && InCellLocation.X >= CollisionOffsetLeft /* && Direction.X > 0.5*/)
                {
                    GravityValue /= 4;
                    canDoubleJump = true;
                    JumpModifier = new Vector2(-5, 0);
                }
                else if (CollisionChecker.HasObjectAtWithTag(GridUtil.GetLeftGrid(GridCoordinates), "SlideWall")
                    && InCellLocation.X <= CollisionOffsetRight /* && Direction.X < -0.5*/)
                {
                    GravityValue /= 4;
                    canDoubleJump = true;
                    JumpModifier = new Vector2(5, 0);
                }
                else
                {
                    JumpModifier = Vector2.Zero;
                }

                if (FallStartedAt == 0)
                {
                    FallStartedAt = (float)gameTime.TotalGameTime.TotalSeconds;
                    canDoubleJump = true;
                }

                if (Config.INCREASING_GRAVITY)
                {
                    t = (float)(gameTime.TotalGameTime.TotalSeconds - FallStartedAt) * Config.GRAVITY_T_MULTIPLIER;
                    Velocity.Y += GravityValue * t * elapsedTime;
                } else
                {
                    Velocity.Y += GravityValue * elapsedTime;
                }
                
                canJump = false;
            }
                
            if (HasGravity && OnGround() /*|| direction.Y < 0*/)
            {
                canJump = true;
                canDoubleJump = false;
                FallStartedAt = 0;
                JumpModifier = Vector2.Zero;
            }

            steps2 = (float)Math.Ceiling(Math.Abs((Velocity.Y + bdy) * elapsedTime));
            step2 = (float)(Velocity.Y + bdy) * elapsedTime / steps2;
            while (steps2 > 0)
            {
                InCellLocation.Y += step2;

                if (HasCollision && InCellLocation.Y > CollisionOffsetBottom && OnGround() && Velocity.Y > 0)
                {
                    if (HasGravity)
                    {
                        Velocity.Y = 0;
                        bdy = 0;
                    }

                    InCellLocation.Y = CollisionOffsetBottom;
                }

                if (HasCollision && InCellLocation.Y < CollisionOffsetTop && CollisionChecker.HasBlockingColliderAt(GridUtil.GetUpperGrid(GridCoordinates)))
                {
                    if (!CollisionChecker.GetColliderAt(GridUtil.GetUpperGrid(GridCoordinates)).HasTag("Platform"))
                    {
                        Velocity.Y = 0;
                        InCellLocation.Y = CollisionOffsetTop;
                    }
                }
                   
                while (InCellLocation.Y > 1) { InCellLocation.Y--; GridCoordinates.Y++; }
                while (InCellLocation.Y < 0) { InCellLocation.Y++; GridCoordinates.Y--; }
                steps2--;
            }

            // workaround for bug when character ends up standing inside a collider or slightly above it
            // when the movement started from below or the fall started from height less than sprite graphics offset
            if (OnGround() && HasGravity && Math.Abs(Velocity.Y) < 0.05)
            {

                if (InCellLocation.Y > CollisionOffsetBottom) {
                    InCellLocation.Y -= 0.09f * elapsedTime * GravityValue;
                }
                if (InCellLocation.Y < CollisionOffsetBottom)
                {
                    InCellLocation.Y += 0.09f * elapsedTime * GravityValue;
                }
                //InCellLocation.Y = CollisionOffsetBottom;
            }

            Velocity.Y *= (float)Math.Pow(Friction, elapsedTime);
            bdy *= (float)Math.Pow(BumpFriction, elapsedTime);
            //rounding stuff
            if (Math.Abs(Velocity.Y) <= 0.0005 * elapsedTime) Velocity.Y = 0;
            if (Math.Abs(bdy) <= 0.0005 * elapsedTime) bdy = 0;
            base.Update(gameTime);
        }

        public override void PostUpdate(GameTime gameTime)
        {
            Position = (GridCoordinates + InCellLocation) * Config.GRID;
            base.PostUpdate(gameTime);
        }

        protected bool OnGround()
        {
            return CollisionChecker.HasBlockingColliderAt(GridUtil.GetBelowGrid(GridCoordinates));
        }

        public void ResetPosition(Vector2 position)
        {
            InCellLocation = Vector2.Zero;
            this.Position = StartPosition = position;
            this.FallStartedAt = 0;
        }
    }
}
