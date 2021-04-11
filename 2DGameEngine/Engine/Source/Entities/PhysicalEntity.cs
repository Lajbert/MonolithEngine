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

        public bool IsOnGround;

        protected float HorizontalFriction = Config.HORIZONTAL_FRICTION;
        protected float VerticalFriction = Config.VERTICAL_FRICTION;
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

        private float previousRotation = 0f;

        public PhysicalEntity(Layer layer, Entity parent, Vector2 startPosition, SpriteFont font = null) : base(layer, parent, startPosition, font)
        {
            Transform = new DynamicTransform(this, startPosition);
            previousPosition = startPosition;
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
                        colliderMarker = AssetUtil.CreateCircle((int)((CircleCollisionComponent)collisionComponent).Radius * 2, Color.Black);
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

            if (previousRotation != Transform.Rotation)
            {
                DrawRotation = MathUtil.LerpRotationDegrees(previousRotation, Transform.Rotation, Globals.FixedUpdateAlpha);
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

            previousRotation = Transform.Rotation;

            if (leftCollider != null)
            {
                if (leftCollider.Velocity.X != 0)
                {
                    if (Velocity.X >= 0)
                    {
                        VelocityX = leftCollider.Velocity.X * (float)(1 / Math.Pow(HorizontalFriction, Globals.FixedUpdateMultiplier));
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
                        VelocityX = rightCollider.Velocity.X * (float)(1 / Math.Pow(HorizontalFriction, Globals.FixedUpdateMultiplier));
                    }
                }
                else if (Velocity.X > 0)
                {
                    VelocityX = 0;
                }
            }

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

            float steps = (float)(Math.Ceiling(Math.Abs((Velocity.X + bump.X) * Globals.FixedUpdateMultiplier) + (Math.Abs((Velocity.Y + bump.Y) * Globals.FixedUpdateMultiplier))) / Config.COLLISION_CHECK_GRID_SIZE);

            if (steps > 0)
            {
                float stepX = (float)((Velocity.X + bump.X) * Globals.FixedUpdateMultiplier) / steps;
                float stepY = (float)((Velocity.Y + bump.Y) * Globals.FixedUpdateMultiplier) / steps;

                while (steps > 0)
                {
                    Transform.InCellLocation.X += stepX;

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

                    // Y

                    Transform.InCellLocation.Y += stepY;

                    if (mountedOn == null && CheckGridCollisions && Transform.InCellLocation.Y > CollisionOffsetBottom && Scene.GridCollisionChecker.HasBlockingColliderAt(Transform.GridCoordinates, Direction.SOUTH)/* && Velocity.Y > 0*/)
                    {
                        if (HasGravity)
                        {
                            if (Velocity.Y > 0)
                            {
                                OnLand(Velocity);
                            }

                            velocity.Y = 0;
                            bump.Y = 0;
                            Transform.InCellLocation.Y = CollisionOffsetBottom;
                        }
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

                    steps--;

                }
            }

            if (HorizontalFriction > 0)
            {
                velocity.X *= (float)Math.Pow(HorizontalFriction, Globals.FixedUpdateMultiplier);
            }

            if (BumpFriction > 0)
            {
                bump.X *= (float)Math.Pow(BumpFriction, Globals.FixedUpdateMultiplier);
            }


            //rounding stuff
            if (Math.Abs(Velocity.X) <= 0.0005 * Globals.FixedUpdateMultiplier) velocity.X = 0;
            if (Math.Abs(bump.X) <= 0.0005 * Globals.FixedUpdateMultiplier) bump.X = 0;

            if (VerticalFriction > 0)
            {
                velocity.Y *= (float)Math.Pow(VerticalFriction, Globals.FixedUpdateMultiplier);
            }
            if (BumpFriction > 0)
            {
                bump.Y *= (float)Math.Pow(BumpFriction, Globals.FixedUpdateMultiplier);
            }

            //rounding stuff
            if (Math.Abs(Velocity.Y) <= 0.0005 * Globals.FixedUpdateMultiplier) velocity.Y = 0;
            if (Math.Abs(bump.Y) <= 0.0005 * Globals.FixedUpdateMultiplier) bump.Y = 0;

            if (Parent == null)
            {
                Transform.Position = (Transform.GridCoordinates + Transform.InCellLocation) * Config.GRID;
            }

            base.FixedUpdate();
        }
        private void ApplyGravity()
        {
            if (Config.INCREASING_GRAVITY)
            {
                float t = (float)(Globals.GameTime.TotalGameTime.TotalSeconds - FallSpeed) * Config.GRAVITY_T_MULTIPLIER;
                velocity.Y += GravityValue * t * Globals.FixedUpdateMultiplier;
            }
            else
            {
                velocity.Y += GravityValue * Globals.FixedUpdateMultiplier * Config.TIME_OFFSET;
            }
        }

        public override void PostUpdate()
        {
            //Position = (Transform.GridCoordinates + Transform.InCellLocation) * Config.GRID;
            
            base.PostUpdate();
        }

        private bool OnGround()
        {
            bool onGround = mountedOn != null || Scene.GridCollisionChecker.HasBlockingColliderAt(Transform.GridCoordinates, Direction.SOUTH) && Transform.InCellLocation.Y == CollisionOffsetBottom && Velocity.Y >= 0;
            if (!onGround && IsOnGround)
            {
                OnLeaveGround();
            }
            IsOnGround = onGround;
            return IsOnGround;
        }

        protected virtual void OnLeaveGround()
        {
            //bump = Vector2.Zero;
        }
        protected virtual void OnLand(Vector2 velocity)
        {
            //bump = Vector2.Zero;
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

                    float xOverlap = Math.Max(0, Math.Min(thisBox.Position.X + thisBox.Width, otherBox.Position.X + otherBox.Width) - Math.Max(thisBox.Position.X, otherBox.Position.X));
                    float yOverlap = Math.Max(0, Math.Min(thisBox.Position.Y + thisBox.Height, otherBox.Position.Y + otherBox.Height) - Math.Max(thisBox.Position.Y, otherBox.Position.Y));

                    if (yOverlap != 0 && yOverlap < xOverlap && thisBox.Position.Y < otherBox.Position.Y)
                    {
                        if (yOverlap > 0 && !OnGround() && velocity.Y > 0)
                        {
                            OnLand(Velocity);
                            VelocityY = 0;
                            mountedOn = otherCollider as PhysicalEntity;
                            FallSpeed = 0;
                            Transform.Y -= yOverlap;
                            Transform.InCellLocation.Y = MathUtil.CalculateInCellLocation(Transform.Position).Y;
                            Transform.GridCoordinates.Y = (int)(Transform.Position.Y / Config.GRID);

                            if (Parent == null)
                            {
                                Transform.Y = (Transform.GridCoordinates.Y + Transform.InCellLocation.Y) * Config.GRID;
                            }
                        }
                    } 
                    else if (xOverlap > 0 && xOverlap < yOverlap)
                    {
                        if (Velocity.X > 0)
                        {
                            VelocityX = 0;
                            rightCollider = otherCollider as PhysicalEntity;
                            Transform.X -= xOverlap;
                            Transform.InCellLocation.X = MathUtil.CalculateInCellLocation(Transform.Position).X;
                            Transform.GridCoordinates.X = (int)(Transform.Position.X / Config.GRID);

                            if (Parent == null)
                            {
                                Transform.X = (Transform.GridCoordinates.X + Transform.InCellLocation.X) * Config.GRID;
                            }
                        }

                        if (Velocity.X < 0)
                        {
                            VelocityX = 0;
                            leftCollider = otherCollider as PhysicalEntity;
                            Transform.X += xOverlap;
                            Transform.InCellLocation.X = MathUtil.CalculateInCellLocation(Transform.Position).X;
                            Transform.GridCoordinates.X = (int)(Transform.Position.X / Config.GRID);

                            if (Parent == null)
                            {
                                Transform.X = (Transform.GridCoordinates.X + Transform.InCellLocation.X) * Config.GRID;
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
