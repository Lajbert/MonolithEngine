using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MonolithEngine
{
    /// <summary>
    /// An entity class with velocity, which provides 
    /// lots of opportunities for physical interactions.
    /// </summary>
    public class PhysicalEntity : Entity
    {

        private Vector2 bump;

        private bool keepBouncing = false;

        public bool IsOnGround;

        public float HorizontalFriction = Config.HORIZONTAL_FRICTION;
        public float VerticalFriction = Config.VERTICAL_FRICTION;
        protected float BumpFriction = Config.BUMP_FRICTION;

        protected float MovementSpeed = Config.CHARACTER_SPEED;

        public float GravityValue = Config.GRAVITY_FORCE;

        protected float FallSpeed { get; set; }

        public bool HasGravity = Config.GRAVITY_ON;

        private Texture2D colliderMarker;

        internal PhysicalEntity MountedOn = null;

        private PhysicalEntity leftCollider = null;
        private PhysicalEntity rightCollider = null;

        private Vector2 previousPosition = Vector2.Zero;

        private float previousRotation = 0f;

        private float stepX;
        private float stepY;

        protected bool CollidesOnGrid { get; private set; }

        private new DynamicTransform Transform;

        public PhysicalEntity(Layer layer, Entity parent = null, Vector2 startPosition = default) : base(layer, parent, startPosition)
        {
            Transform = new DynamicTransform(this, startPosition);
            base.Transform = Transform;
            previousPosition = startPosition;
            CheckGridCollisions = true;
            Active = true;
            ResetPosition(startPosition);
            Scene.CollisionEngine.OnCollisionProfileChanged(this);
        }

        override public void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
#if DEBUG
            ICollisionComponent collisionComponent = GetComponent<ICollisionComponent>();
            if (DEBUG_SHOW_PIVOT)
            {
                //spriteBatch.DrawString(font, "Y: " + Transform.VelocityY, DrawPosition, Color.White);
            }

            if (DEBUG_SHOW_COLLIDER && collisionComponent != null)
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
        }

        public void Bump(Vector2 direction, bool keepBouncing = false)
        {
            bump = direction;
            FallSpeed = 0;
            stepY = 0;
            this.keepBouncing = keepBouncing;
        }

        public override void Update()
        {
            if (previousPosition == Transform.Position || Config.FIXED_UPDATE_FPS == VideoConfiguration.FRAME_LIMIT || Config.FIXED_UPDATE_FPS == 0)
            {
                DrawPosition = Transform.Position;
                //DrawPosition = MathUtil.IsoFrom2D(Transform.Position);
            }
            else
            {
                DrawPosition = Vector2.Lerp(previousPosition, Transform.Position, Globals.FixedUpdateAlpha);
                //DrawPosition = Vector2.Lerp(MathUtil.IsoFrom2D(previousPosition), MathUtil.IsoFrom2D(Transform.Position), Globals.FixedUpdateAlpha);
            }

            if (previousRotation != Transform.Rotation)
            {
                DrawRotation = MathUtil.LerpRotationDegrees(previousRotation, Transform.Rotation, Globals.FixedUpdateAlpha);
            }

            base.Update();
        }

        public override void PreFixedUpdate()
        {
            base.PreFixedUpdate();
        }

        public override void FixedUpdate()
        {
            CollidesOnGrid = false;

            if (Transform.Velocity != Vector2.Zero)
            {
                Transform.Velocity.Normalize();
            }

            if (Parent == null)
            {
                previousPosition = Transform.Position;
            }
            else
            {
                previousPosition = (Parent as PhysicalEntity).previousPosition + Transform.PositionWithoutParent;
            }

            previousRotation = Transform.Rotation;

            if (leftCollider != null)
            {
                if (leftCollider.Transform.VelocityX != 0)
                {
                    if (Transform.VelocityX >= 0)
                    {
                        Transform.VelocityX = leftCollider.Transform.VelocityX * (float)(1 / Math.Pow(HorizontalFriction, Globals.FixedUpdateMultiplier));
                    }
                }
                else if (Transform.VelocityX < 0)
                {
                    Transform.VelocityX = 0;
                }
            }

            if (rightCollider != null)
            {
                if (rightCollider.Transform.VelocityX != 0)
                {
                    if (Transform.VelocityX <= 0)
                    {
                        Transform.VelocityX = rightCollider.Transform.VelocityX * (float)(1 / Math.Pow(HorizontalFriction, Globals.FixedUpdateMultiplier));
                    }
                }
                else if (Transform.VelocityX > 0)
                {
                    Transform.VelocityX = 0;
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

            float steps = (float)(Math.Ceiling(Math.Abs((Transform.VelocityX + bump.X) * Globals.FixedUpdateMultiplier) + (Math.Abs((Transform.VelocityY + bump.Y) * Globals.FixedUpdateMultiplier))) / Config.DYNAMIC_COLLISION_CHECK_FREQUENCY);
            IsCollisionCheckedInCurrentLoop = steps > 0;
            if (steps > 0)
            {
                stepX = (float)((Transform.VelocityX + bump.X) * Globals.FixedUpdateMultiplier) / steps;
                stepY = (float)((Transform.VelocityY + bump.Y) * Globals.FixedUpdateMultiplier) / steps;

                while (steps > 0)
                {

                    Transform.InCellLocation.X += stepX;

                    if (CheckGridCollisions && Transform.InCellLocation.X > CollisionOffsetLeft && Scene.GridCollisionChecker.HasBlockingColliderAt(this, Direction.EAST))
                    {
                        Transform.InCellLocation.X = CollisionOffsetLeft;
                        CollidesOnGrid = true;
                    }

                    if (CheckGridCollisions && Transform.InCellLocation.X < CollisionOffsetRight && Scene.GridCollisionChecker.HasBlockingColliderAt(this, Direction.WEST))
                    {
                        Transform.InCellLocation.X = CollisionOffsetRight;
                        CollidesOnGrid = true;
                    }

                    while (Transform.InCellLocation.X > 1)
                    {
                        Transform.InCellLocation.X--;
                        Transform.gridCoordinates.X++;
                    }
                    while (Transform.InCellLocation.X < 0)
                    {
                        Transform.InCellLocation.X++;
                        Transform.gridCoordinates.X--;
                    }

                    // Y

                    Transform.InCellLocation.Y += stepY;

                    if (MountedOn == null && CheckGridCollisions && Transform.InCellLocation.Y > CollisionOffsetBottom && Scene.GridCollisionChecker.HasBlockingColliderAt(this, Direction.SOUTH)/* && Transform.VelocityY > 0*/)
                    {
                        //if (HasGravity)
                        {
                            if (Transform.VelocityY > 0)
                            {
                                OnLand(Transform.Velocity);
                            }

                            Transform.VelocityY = 0;
                            if (!keepBouncing)
                            {
                                bump.Y = 0;
                            }
                            Transform.InCellLocation.Y = CollisionOffsetBottom;
                            CollidesOnGrid = true;
                        }
                    }

                    if (CheckGridCollisions && Transform.InCellLocation.Y < CollisionOffsetTop && Scene.GridCollisionChecker.HasBlockingColliderAt(this, Direction.NORTH))
                    {
                        Transform.VelocityY = 0;
                        bump.Y = 0;
                        Transform.InCellLocation.Y = CollisionOffsetTop;
                        CollidesOnGrid = true;
                    }

                    while (Transform.InCellLocation.Y > 1)
                    {
                        Transform.InCellLocation.Y--;
                        Transform.gridCoordinates.Y++;
                    }
                    while (Transform.InCellLocation.Y < 0)
                    {
                        Transform.InCellLocation.Y++;
                        Transform.gridCoordinates.Y--;
                    }

                    SetPosition();
                    Scene.CollisionEngine.CheckCollisions(this);

                    steps--;

                    if (stepX == 0 && stepY == 0)
                    {
                        steps = 0;
                    }

                }
            }

            if (HorizontalFriction > 0)
            {
                Transform.VelocityX *= (float)Math.Pow(HorizontalFriction, Globals.FixedUpdateMultiplier);
            }

            if (BumpFriction > 0)
            {
                bump.X *= (float)Math.Pow(BumpFriction, Globals.FixedUpdateMultiplier * 0.1);
            }


            //rounding stuff
            if (Math.Abs(Transform.VelocityX) <= 0.0005 * Globals.FixedUpdateMultiplier) Transform.VelocityX = 0;
            if (Math.Abs(bump.X) <= 0.0005 * Globals.FixedUpdateMultiplier) bump.X = 0;

            if (VerticalFriction > 0)
            {
                Transform.VelocityY *= (float)Math.Pow(VerticalFriction, Globals.FixedUpdateMultiplier);
            }
            if (BumpFriction > 0)
            {
                bump.Y *= (float)Math.Pow(BumpFriction, Globals.FixedUpdateMultiplier * 0.1);
            }

            //rounding stuff
            if (Math.Abs(Transform.VelocityY) <= 0.0005 * Globals.FixedUpdateMultiplier) Transform.VelocityY = 0;
            if (Math.Abs(bump.Y) <= 0.1 * Globals.FixedUpdateMultiplier) bump.Y = 0;

            if (Parent == null)
            {
                SetPosition();
            }


            base.FixedUpdate();
        }

        private void SetPosition()
        {
            Transform.PositionWithoutParent = (Transform.gridCoordinates + Transform.InCellLocation) * Config.GRID;
        }

        private void ApplyGravity()
        {
            if (Config.INCREASING_GRAVITY)
            {
                float t = (float)(Globals.GameTime.TotalGameTime.TotalSeconds - FallSpeed) * Config.GRAVITY_T_MULTIPLIER;
                Transform.VelocityY += GravityValue * t * Globals.FixedUpdateMultiplier;
            }
            else
            {
                Transform.VelocityY += GravityValue * Globals.FixedUpdateMultiplier * Config.TIME_OFFSET;
            }
        }

        public override void PostUpdate()
        {
            //Position = (Transform.GridCoordinates + Transform.InCellLocation) * Config.GRID;
            
            base.PostUpdate();
        }

        private bool OnGround()
        {
            bool onGround = MountedOn != null || Scene.GridCollisionChecker.HasBlockingColliderAt(this, Direction.SOUTH) && Transform.InCellLocation.Y == CollisionOffsetBottom && Transform.VelocityY >= 0;
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
            PositionEntity(otherCollider, allowOverlap);
            base.HandleCollisionStart(otherCollider, allowOverlap);
        }

        private void PositionEntity(IGameObject otherCollider, bool allowOverlap)
        {
            if (!allowOverlap && Parent == null)
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
                        if (yOverlap > 0 && !OnGround() && Transform.VelocityY > 0)
                        {
                            stepY = 0;
                            OnLand(Transform.Velocity);
                            Transform.VelocityY = 0;
                            MountedOn = otherCollider as PhysicalEntity;
                            float tmp = Transform.Position.X - MountedOn.Transform.Position.X;
                            Parent = MountedOn;
                            Transform.Position = new Vector2(tmp, 0);
                            bump = Vector2.Zero;
                            FallSpeed = 0;
                        }
                    } 
                    else if (xOverlap > 0 && xOverlap < yOverlap)
                    {
                        if (Transform.VelocityX > 0)
                        {
                            stepX = 0;
                            Transform.VelocityX = 0;
                            rightCollider = otherCollider as PhysicalEntity;
                            Transform.X -= xOverlap;
                        }

                        if (Transform.VelocityX < 0)
                        {
                            stepX = 0;
                            Transform.VelocityX = 0;
                            leftCollider = otherCollider as PhysicalEntity;
                            Transform.X += xOverlap;
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
        }

        internal sealed override void HandleCollisionEnd(IGameObject otherCollider)
        {
            if (MountedOn != null && otherCollider.Equals(MountedOn))
            {
                Transform.Velocity += MountedOn.Transform.Velocity;
                MountedOn = null;
                Parent = null;
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
            Transform.InCellLocation = MathUtil.CalculateInCellLocation(position);
            Transform.GridCoordinates = new Vector2((int)(position.X / Config.GRID), (int)(position.Y / Config.GRID));
            Transform.Position = position;
            FallSpeed = 0;
        }

        public override void Destroy()
        {
            CheckGridCollisions = false;
            Transform.Velocity = Vector2.Zero;
            bump = Vector2.Zero;
            base.Destroy();
        }

        public bool IsMovingAtLeast(float speed)
        {
            return Math.Abs(Transform.VelocityX) >= speed || Math.Abs(Transform.VelocityY) >= speed;
        }

        public Vector2 GetVelocity()
        {
            return Transform.Velocity;
        }

        public void AddForce(Vector2 force)
        {
            Transform.Velocity += force;
        }
        

        public void SetVelocity(Vector2 velocity)
        {
            Transform.Velocity = velocity;
        }

        public void AddVelocity(Vector2 velocity)
        {
            Transform.Velocity += velocity;
        }

        public bool IsAtRest()
        {
            return Transform.Velocity == Vector2.Zero && bump == Vector2.Zero;
        }

        public void CancelVelocities()
        {
            Transform.Velocity = Vector2.Zero;
            bump = Vector2.Zero;
            stepX = 0;
            stepY = 0;
        }
    }
}
