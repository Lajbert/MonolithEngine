using GameEngine2D.Engine.Source.Entities;
using GameEngine2D.Engine.Source.Entities.Controller;
using GameEngine2D.Engine.Source.Layer;
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
    public class ControllableEntity : Entity, IGravityApplicable
    {

        protected float CollisionOffsetLeft = 0f;
        protected float CollisionOffsetRight = 0f;
        protected float CollisionOffsetBottom = 0f;
        protected float CollisionOffsetTop = 0f;


        private float bdx = 0f;
        private float bdy = 0f;

        protected UserInputController UserInput;

        protected bool canJump = true;
        protected bool doubleJump = false;
        protected float elapsedTime;
        private float steps;
        private float step;
        private float steps2;
        private float step2;
        private float t;
        public bool HasGravity { get; set; }  = Config.GRAVITY_ON;

        public float JumpStart { get; set; }

        protected GameTime gameTime;

        protected FaceDirection CurrentFaceDirection { get; set; } = Engine.Source.Entities.FaceDirection.RIGHT;

        public ControllableEntity(AbstractLayer layer, Entity parent, Vector2 startPosition, Texture2D texture = null, SpriteFont font = null) : base(layer, parent, startPosition, texture, font)
        {
            ResetPosition(startPosition);
        }

        override public void Draw(GameTime gameTime)
        {

            this.gameTime = gameTime;

            elapsedTime = TimeUtil.GetElapsedTime(gameTime);


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
            SpriteBatch.Begin();
            SpriteBatch.DrawString(font, InCellLocation.X + " : " + InCellLocation.Y, new Vector2(10,10), Color.White);
            SpriteBatch.End();
#endif
            
            base.Draw(gameTime);
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

            this.gameTime = gameTime;

            steps = (float)Math.Ceiling(Math.Abs((Direction.X + bdx) * elapsedTime));
            step = (float)(Direction.X + bdx) * elapsedTime / steps;
            while (steps > 0)
            {
                InCellLocation.X += step;

                if (HasCollision && InCellLocation.X >= CollisionOffsetLeft && Scene.Instance.HasColliderAt(GridUtil.GetRightGrid(GridCoordinates)))
                //if (HasCollision && InCellLocation.X >= CollisionOffsetLeft && (Scene.Instance.HasColliderAt(GridUtil.GetRightGrid(GridCoordinates)) || (/*!HasGravity && */!OnGround() && Scene.Instance.HasColliderAt(new Vector2(GridCoordinates.X + 1, GridCoordinates.Y + 1)))))
                //if (HasCollision && InCellLocation.X >= CollisionOffsetLeft && (Scene.Instance.HasColliderAt(GridUtil.GetRightGrid(GridCoordinates)) || (/*!HasGravity && */!Scene.Instance.HasColliderAt(GridUtil.GetBelowGrid(GridCoordinates)) && Scene.Instance.HasColliderAt(new Vector2(GridCoordinates.X + 1, GridCoordinates.Y + 1)))))
                {
                    //Direction.X = 0;
                    //bdx = 0;
                    InCellLocation.X = CollisionOffsetLeft;
                }

                if (HasCollision && InCellLocation.X <= CollisionOffsetRight && Scene.Instance.HasColliderAt(GridUtil.GetLeftGrid(GridCoordinates)))
                //if (HasCollision && InCellLocation.X <= CollisionOffsetRight && (Scene.Instance.HasColliderAt(GridUtil.GetLeftGrid(GridCoordinates)) || (/*!HasGravity && */!Scene.Instance.HasColliderAt(GridUtil.GetBelowGrid(GridCoordinates)) && Scene.Instance.HasColliderAt(new Vector2(GridCoordinates.X - 1, GridCoordinates.Y + 1)))))
                {
                    //Direction.X = 0;
                    //bdx = 0;
                    InCellLocation.X = CollisionOffsetRight;
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
                canJump = true;
                doubleJump = true;
                JumpStart = 0;
            }

            steps2 = (float)Math.Ceiling(Math.Abs((Direction.Y + bdy) * elapsedTime));
            step2 = (float)(Direction.Y + bdy) * elapsedTime / steps2;
            while (steps2 > 0)
            {
                InCellLocation.Y += step2;

                //if (HasCollision && InCellLocation.Y > CollisionOffsetBottom && (Scene.Instance.HasColliderAt(GridUtil.GetBelowGrid(GridCoordinates)) || Scene.Instance.HasColliderAt(GridUtil.GetRightBelowGrid(GridCoordinates))))
                if (HasCollision && InCellLocation.Y > CollisionOffsetBottom && OnGround())
                {
                    if (HasGravity)
                    {
                        Direction.Y = 0;
                        bdy = 0;
                    }

                    InCellLocation.Y = CollisionOffsetBottom;
                }

                if (HasCollision && InCellLocation.Y < CollisionOffsetTop && Scene.Instance.HasColliderAt(GridUtil.GetUpperGrid(GridCoordinates)))
                {
                    InCellLocation.Y = CollisionOffsetTop;
                }
                   
                while (InCellLocation.Y > 1) { InCellLocation.Y--; GridCoordinates.Y++; }
                while (InCellLocation.Y < 0) { InCellLocation.Y++; GridCoordinates.Y--; }
                steps2--;
            }
            Direction.Y *= (float)Math.Pow(Config.FRICTION, elapsedTime);
            bdy *= (float)Math.Pow(Config.BUMB_FRICTION, elapsedTime);
            if (Math.Abs(Direction.Y) <= 0.0005 * elapsedTime) Direction.Y = 0;
            if (Math.Abs(bdy) <= 0.0005 * elapsedTime) bdy = 0;
            base.Update(gameTime);
        }

        public override void PostUpdate(GameTime gameTime)
        {
            Position = (GridCoordinates + InCellLocation) * Config.GRID;
            base.PostUpdate(gameTime);
        }

        public float GetGravityConstant()
        {
            return Config.GRAVITY_FORCE;
        }

        private bool OnGround()
        {
            return Scene.Instance.HasColliderAt(GridUtil.GetBelowGrid(GridCoordinates))/* && InCellLocation.Y == 1 && Direction.Y >= 0*/;
            //return Scene.Instance.HasColliderAt(GridUtil.GetBelowGrid(GridCoordinates)) || Scene.Instance.HasColliderAt(GridUtil.GetRightBelowGrid(GridCoordinates))/* && InCellLocation.Y == 1 && Direction.Y >= 0*/;
        }

        public void ResetPosition(Vector2 position)
        {
            InCellLocation = Vector2.Zero;
            this.Position = StartPosition = position;
            this.JumpStart = 0;
        }
    }
}
