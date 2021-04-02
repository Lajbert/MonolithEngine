using MonolithEngine.Engine.Source.Entities;
using MonolithEngine.Engine.Source.Entities.Abstract;
using MonolithEngine.Engine.Source.Entities.Controller;
using MonolithEngine.Engine.Source.Entities.Interfaces;
using MonolithEngine.Engine.Source.Entities.Transform;
using MonolithEngine.Engine.Source.Global;
using MonolithEngine.Engine.Source.Physics;
using MonolithEngine.Engine.Source.Physics.Collision;
using MonolithEngine.Engine.Source.Physics.Interface;
using MonolithEngine.Engine.Source.Util;
using MonolithEngine.Entities;
using MonolithEngine.Entities.Interfaces;
using MonolithEngine.Global;
using MonolithEngine.Source;
using MonolithEngine.Source.GridCollision;
using MonolithEngine.Source.Util;
using MonolithEngine.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using MonolithEngine.Engine.Source.MyGame;

namespace MonolithEngine
{
    public class PhysicalEntity : Entity
    {

        private Vector2 bump;

        protected UserInputController UserInput;

        private float steps;
        private float step;
        private float steps2;
        private float step2;
        private float t;

        private Vector2 velocity = Vector2.Zero;

        public Vector2 Velocity
        {
            get {
                if (mountedOn == null)
                {
                    return velocity;
                }
                return mountedOn.Velocity + velocity;
            }

            set => velocity = value;
        }

        public float VelocityX {
            get
            {
                /*if (MountedOn == null)
                {
                    return velocity.X;
                }
                return MountedOn.Velocity.X + velocity.X;*/
                return velocity.X;
            }

            set => velocity.X = value;
        }

        public float VelocityY
        {
            get
            {
                /*if (MountedOn == null)
                {
                    return velocity.Y;
                }
                return MountedOn.Velocity.Y + velocity.Y;*/
                return velocity.Y;
            }

            set => velocity.Y = value;
        }

        public bool IsOnGround
        {
            get => OnGround();
        }

        protected float Friction = Config.FRICTION;
        protected float BumpFriction = Config.BUMP_FRICTION;

        protected float MovementSpeed = Config.CHARACTER_SPEED;

        public float GravityValue = Config.GRAVITY_FORCE;

        protected float FallSpeed { get; set; }

        public bool HasGravity = Config.GRAVITY_ON;

        private Texture2D colliderMarker;

        private PhysicalEntity mountedOn = null;

        private PhysicalEntity leftCollider = null;
        private PhysicalEntity rightCollider = null;

        private Vector2 previousPosition = Vector2.Zero;

        public PhysicalEntity(Layer layer, Entity parent, Vector2 startPosition, SpriteFont font = null) : base(layer, parent, startPosition, font)
        {
            Transform = new DynamicTransform(this, startPosition);
            CheckGridCollisions = true;
            Active = true;
            ResetPosition(startPosition);
            Scene.CollisionEngine.OnCollisionProfileChanged(this);
        }

        override public void Draw(SpriteBatch spriteBatch)
        {
#if DEBUG
            ICollisionComponent collisionComponent = GetComponent<ICollisionComponent>();
            if (DEBUG_SHOW_PIVOT)
            {
                //spriteBatch.DrawString(font, "Y: " + Velocity.Y, DrawPosition, Color.White);
            }

            if (DEBUG_SHOW_COLLIDER)
            {
                (collisionComponent as AbstractCollisionComponent).DEBUG_DISPLAY_COLLISION = true;
                if (colliderMarker == null)
                {
                    if (collisionComponent is CircleCollisionComponent)
                    {
                        (collisionComponent as AbstractCollisionComponent).DEBUG_DISPLAY_COLLISION = true;
                        colliderMarker = TextureUtil.CreateCircle((int)((CircleCollisionComponent)collisionComponent).Radius * 2, Color.Black);
                    }
                }
                if (collisionComponent != null && collisionComponent is CircleCollisionComponent)
                {
                    spriteBatch.Draw(colliderMarker, ((CircleCollisionComponent)collisionComponent).Position - new Vector2(((CircleCollisionComponent)collisionComponent).Radius, ((CircleCollisionComponent)collisionComponent).Radius), Color.White);
                }
            }
#endif
            base.Draw(spriteBatch);
        }

        public override void PreUpdate()
        {
            if (UserInput != null)
            {
                UserInput.Update();
            }

            base.PreUpdate();
        }

        protected virtual void OnLand()
        {
            //bump = Vector2.Zero;
        }

        public void Bump(Vector2 direction)
        {
            bump = direction;
        }

        public override void Update()
        {
            if (previousPosition == Transform.Position || Config.FIXED_UPDATE_FPS == VideoConfiguration.FRAME_LIMIT || Config.FIXED_UPDATE_FPS == 0)
            {
                DrawPosition = Transform.Position;
            }
            else
            {
                DrawPosition = Vector2.Lerp(previousPosition, Transform.Position, Globals.FixedUpdateAlpha);
            }

            base.Update();
        }

        public override void FixedUpdate()
        {

            if (Velocity != Vector2.Zero)
            {
                Velocity.Normalize();
            }

            previousPosition = Transform.Position;

            float gameTime = (float)Globals.FixedUpdateMultiplier * 0.01f;

            if (leftCollider != null)
            {
                if (leftCollider.Velocity.X != 0)
                {
                    if (Velocity.X >= 0)
                    {
                        VelocityX = leftCollider.Velocity.X * (float)(1 / Math.Pow(Friction, gameTime));
                    }
                }
                else if (Velocity.X < 0)
                {
                    VelocityX = 0;
                }
            }

            if (rightCollider != null)
            {
                if (rightCollider.Velocity.X != 0)
                {
                    if (Velocity.X <= 0)
                    {
                        VelocityX = rightCollider.Velocity.X * (float)(1 / Math.Pow(Friction, gameTime));
                    }
                }
                else if (Velocity.X > 0)
                {
                    VelocityX = 0;
                }
            }

            steps = (float)Math.Ceiling(Math.Abs((Velocity.X + bump.X) * gameTime));
            step = (float)(Velocity.X + bump.X) * gameTime / steps;
            while (steps > 0)
            {
                Transform.InCellLocation.X += step;

                if (CheckGridCollisions && Transform.InCellLocation.X > CollisionOffsetLeft && Scene.GridCollisionChecker.HasBlockingColliderAt(Transform.GridCoordinates, Direction.EAST))
                {
                    Transform.InCellLocation.X = CollisionOffsetLeft;
                }

                if (CheckGridCollisions && Transform.InCellLocation.X < CollisionOffsetRight && Scene.GridCollisionChecker.HasBlockingColliderAt(Transform.GridCoordinates, Direction.WEST))
                {
                    Transform.InCellLocation.X = CollisionOffsetRight;
                }

                while (Transform.InCellLocation.X > 1)
                {
                    Transform.InCellLocation.X--;
                    Transform.GridCoordinates.X++;
                }
                while (Transform.InCellLocation.X < 0)
                {
                    Transform.InCellLocation.X++;
                    Transform.GridCoordinates.X--;
                }
                steps--;
            }
            if (Friction > 0)
            {
                velocity.X *= (float)Math.Pow(Friction, gameTime);
            }

            if (BumpFriction > 0)
            {
                bump.X *= (float)Math.Pow(BumpFriction, gameTime);
            }


            //rounding stuff
            if (Math.Abs(Velocity.X) <= 0.0005 * gameTime) velocity.X = 0;
            if (Math.Abs(bump.X) <= 0.0005 * gameTime) bump.X = 0;

            // Y
            if (HasGravity && !OnGround())
            {
                if (FallSpeed == 0)
                {
                    FallSpeed = (float)Globals.GameTime.TotalGameTime.TotalSeconds;
                }
                ApplyGravity();
            }

            if (OnGround())
            {
                FallSpeed = 0;
            }

            steps2 = (float)Math.Ceiling(Math.Abs((Velocity.Y + bump.Y) * gameTime));
            step2 = (float)(Velocity.Y + bump.Y) * gameTime / steps2;
            while (steps2 > 0)
            {
                Transform.InCellLocation.Y += step2;

                if (CheckGridCollisions && Transform.InCellLocation.Y > CollisionOffsetBottom && Scene.GridCollisionChecker.HasBlockingColliderAt(Transform.GridCoordinates, Direction.SOUTH)/* && Velocity.Y > 0*/)
                {
                    if (HasGravity)
                    {
                        velocity.Y = 0;
                        bump.Y = 0;
                        Transform.InCellLocation.Y = CollisionOffsetBottom;
                    }
                    OnLand();

                }

                if (CheckGridCollisions && Transform.InCellLocation.Y < CollisionOffsetTop && Scene.GridCollisionChecker.HasBlockingColliderAt(Transform.GridCoordinates, Direction.NORTH))
                {
                    velocity.Y = 0;
                    Transform.InCellLocation.Y = CollisionOffsetTop;
                }

                while (Transform.InCellLocation.Y > 1)
                {
                    Transform.InCellLocation.Y--;
                    Transform.GridCoordinates.Y++;
                }
                while (Transform.InCellLocation.Y < 0)
                {
                    Transform.InCellLocation.Y++;
                    Transform.GridCoordinates.Y--;
                }
                steps2--;
            }

            if (Friction > 0)
            {
                velocity.Y *= (float)Math.Pow(Friction, gameTime);
            }
            if (BumpFriction > 0)
            {
                bump.Y *= (float)Math.Pow(BumpFriction, gameTime);
            }

            //rounding stuff
            if (Math.Abs(Velocity.Y) <= 0.0005 * gameTime) velocity.Y = 0;
            if (Math.Abs(bump.Y) <= 0.0005 * gameTime) bump.Y = 0;

            if (Parent == null)
            {
                /*if (Velocity.X == 0 && velocityAtUpdateStart.X != Velocity.X)
                {
                    Transform.X = (int)((Transform.GridCoordinates.X + Transform.InCellLocation.X) * Config.GRID);
                } else
                {
                    Transform.X = (Transform.GridCoordinates.X + Transform.InCellLocation.X) * Config.GRID;
                }

                if (Velocity.Y == 0 && velocityAtUpdateStart.Y != Velocity.Y)
                {
                    Transform.Y = (int)((Transform.GridCoordinates.Y + Transform.InCellLocation.Y) * Config.GRID);
                }
                else
                {
                    Transform.Y = (Transform.GridCoordinates.Y + Transform.InCellLocation.Y) * Config.GRID;
                }*/

                //Transform.X = (int)((Transform.GridCoordinates.X + Transform.InCellLocation.X) * Config.GRID);
                //Transform.Y = (int)((Transform.GridCoordinates.Y + Transform.InCellLocation.Y) * Config.GRID);

                Transform.X = (Transform.GridCoordinates.X + Transform.InCellLocation.X) * Config.GRID;
                Transform.Y = (Transform.GridCoordinates.Y + Transform.InCellLocation.Y) * Config.GRID;
            }

            base.FixedUpdate();
        }
        private void ApplyGravity()
        {
            if (Config.INCREASING_GRAVITY)
            {
                t = (float)(Globals.GameTime.TotalGameTime.TotalSeconds - FallSpeed) * Config.GRAVITY_T_MULTIPLIER;
                velocity.Y += GravityValue * t * (float)Globals.FixedUpdateMultiplier * 0.01f;
            }
            else
            {
                velocity.Y += GravityValue * (float)Globals.GameTime.ElapsedGameTime.TotalSeconds * Config.TIME_OFFSET; ;
            }
        }

        public override void PostUpdate()
        {
            //Position = (Transform.GridCoordinates + Transform.InCellLocation) * Config.GRID;
            
            base.PostUpdate();
        }

        private bool OnGround()
        {
            return mountedOn != null || Scene.GridCollisionChecker.HasBlockingColliderAt(Transform.GridCoordinates, Direction.SOUTH) && Transform.InCellLocation.Y == CollisionOffsetBottom && Velocity.Y >= 0;
        }

        internal sealed override void HandleCollisionStart(IGameObject otherCollider, bool allowOverlap)
        {
            if (!allowOverlap)
            {
                if (!(otherCollider is Entity) || (otherCollider as Entity).GetCollisionComponent() == null)
                {
                    return;
                }

                ICollisionComponent thisCollisionComp = GetCollisionComponent();
                ICollisionComponent otherCollisionComp = (otherCollider as Entity).GetCollisionComponent();

                if (thisCollisionComp is BoxCollisionComponent && otherCollisionComp is BoxCollisionComponent)
                {

                    BoxCollisionComponent thisBox = thisCollisionComp as BoxCollisionComponent;
                    BoxCollisionComponent otherBox = otherCollisionComp as BoxCollisionComponent;

                    float distanceX = thisBox.Position.X - otherBox.Position.X;
                    float distanceY = thisBox.Position.Y - otherBox.Position.Y;

                    if (-distanceY < thisBox.Height && !OnGround() && velocity.Y > 0)
                    {
                        if (otherBox.Position.Y > thisBox.Position.Y)
                        {
                            VelocityY = 0;
                            //HasGravity = false;
                            mountedOn = otherCollider as PhysicalEntity;
                            FallSpeed = 0;
                            while (-distanceY < thisBox.Height - 1)
                            {
                                Transform.InCellLocation.Y -= 0.01f;

                                while (Transform.InCellLocation.Y > 1)
                                {
                                    Transform.InCellLocation.Y--;
                                    Transform.GridCoordinates.Y++;
                                }
                                while (Transform.InCellLocation.Y < 0)
                                {
                                    Transform.InCellLocation.Y++;
                                    Transform.GridCoordinates.Y--;
                                }

                                if (Parent == null)
                                {
                                    Transform.Y = (int)((Transform.GridCoordinates.Y + Transform.InCellLocation.Y) * Config.GRID);
                                }
                                distanceY = thisBox.Position.Y - otherBox.Position.Y;
                            }
                        }
                    }
                    /*if (distanceY < otherBox.Height && !OnGround())
                    {
                        if (otherBox.Position.Y < thisBox.Position.Y)
                        {
                            while (distanceY < otherBox.Height)
                            {

                                Transform.InCellLocation.Y += 0.01f;

                                while (Transform.InCellLocation.Y > 1)
                                {
                                    Transform.InCellLocation.Y--;
                                    Transform.GridCoordinates.Y++;
                                }
                                while (Transform.InCellLocation.Y < 0)
                                {
                                    Transform.InCellLocation.Y++;
                                    Transform.GridCoordinates.Y--;
                                }

                                if (Parent == null)
                                {
                                    Transform.Y = (int)((Transform.GridCoordinates.Y + Transform.InCellLocation.Y) * Config.GRID);
                                }
                                Logger.Info("Y modified downwards!");
                                distanceY = thisBox.Position.Y - otherBox.Position.Y;
                            }
                        }
                    }*/

                    if (-distanceX < thisBox.Width)
                    {
                        if (thisBox.Position.X < otherBox.Position.X && mountedOn == null && (velocity.X > 0 || (otherCollider as PhysicalEntity).Velocity.X != 0))
                        {
                            VelocityX = 0;
                            rightCollider = otherCollider as PhysicalEntity;
                            while (-distanceX < thisBox.Width - 1)
                            {
                                Transform.InCellLocation.X -= 0.01f;

                                while (Transform.InCellLocation.X > 1)
                                {
                                    Transform.InCellLocation.X--;
                                    Transform.GridCoordinates.X++;
                                }
                                while (Transform.InCellLocation.X < 0)
                                {
                                    Transform.InCellLocation.X++;
                                    Transform.GridCoordinates.X--;
                                }

                                if (Parent == null)
                                {
                                    Transform.X = (int)((Transform.GridCoordinates.X + Transform.InCellLocation.X) * Config.GRID);
                                }

                                distanceX = thisBox.Position.X - otherBox.Position.X;
                            }
                        }
                    }

                    if (distanceX < otherBox.Width)
                    {
                        if (thisBox.Position.X > otherBox.Position.X && mountedOn == null && (velocity.X < 0 || (otherCollider as PhysicalEntity).Velocity.X != 0))
                        {
                            VelocityX = 0;
                            leftCollider = otherCollider as PhysicalEntity;
                            while (distanceX < otherBox.Width - 1)
                            {
                                Transform.InCellLocation.X += 0.01f;

                                while (Transform.InCellLocation.X > 1)
                                {
                                    Transform.InCellLocation.X--;
                                    Transform.GridCoordinates.X++;
                                }
                                while (Transform.InCellLocation.X < 0)
                                {
                                    Transform.InCellLocation.X++;
                                    Transform.GridCoordinates.X--;
                                }

                                if (Parent == null)
                                {
                                    Transform.X = (int)((Transform.GridCoordinates.X + Transform.InCellLocation.X) * Config.GRID);
                                }

                                distanceX = thisBox.Position.X - otherBox.Position.X;
                            }
                        }
                    }
                }
                else if (thisCollisionComp is CircleCollisionComponent && otherCollisionComp is CircleCollisionComponent)
                {
                    throw new Exception("Non-overlapping collision type is not implemented between the current colliders");
                }
                else
                {
                    throw new Exception("Non-overlapping collision type is not implemented between the current colliders");
                }
            }

            base.HandleCollisionStart(otherCollider, allowOverlap);
        }

        internal sealed override void HandleCollisionEnd(IGameObject otherCollider)
        {
            if (mountedOn != null && otherCollider.Equals(mountedOn))
            {
                Velocity += mountedOn.Velocity;
                mountedOn = null;
                //HasGravity = true;
            }

            if (leftCollider != null && otherCollider.Equals(leftCollider)) {
                leftCollider = null;
            }

            if (rightCollider != null && otherCollider.Equals(rightCollider))
            {
                rightCollider = null;
            }

            /*foreach (string tag in (otherCollider as Entity).GetTags())
            {
                if (GetCollidesAgainst().ContainsKey(tag) && !GetCollidesAgainst()[tag])
                {
                    HasGravity = true;
                    break;
                }
            }*/
            base.HandleCollisionEnd(otherCollider);
        }

        public void ResetPosition(Vector2 position)
        {
            Transform.InCellLocation = new Vector2(0.5f, 1f);
            //Transform.InCellLocation = Vector2.Zero;
            //UpdateInCellCoord();
            Transform.Position = position;
            FallSpeed = 0;
        }

        public override void Destroy()
        {
            CheckGridCollisions = false;
            Velocity = Vector2.Zero;
            bump = Vector2.Zero;
            base.Destroy();
        }

        public bool IsMovingAtLeast(float speed)
        {
            return Math.Abs(Velocity.X) >= speed || Math.Abs(Velocity.Y) >= speed;
        }

        public Vector2 GetVelocity()
        {
            return Velocity;
        }

        public void AddForce(Vector2 force)
        {
            Velocity += force;
        }
        

        public void SetVelocity(Vector2 velocity)
        {
            Velocity = velocity;
        }

        public void AddVelocity(Vector2 velocity)
        {
            Velocity += velocity;
        }
    }
}
