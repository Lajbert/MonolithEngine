using MonolithEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace ForestPlatformerExample
{
    class Hero : PhysicalEntity
    {

        private readonly float JUMP_RATE = 0.1f;
        private readonly float SLIDE_FORCE = 1f;

        private static double lastJump = 0f;
        private bool doubleJumping = false;

        private bool canJump = true;
        private bool canDoubleJump = false;

        private Vector2 jumpModifier = Vector2.Zero;

        private bool canAttack = true;

        private float climbSpeed = Config.CHARACTER_SPEED / 2;

        public Fist fist;

        private bool isCarryingItem = false;

        private IMovableItem overlappingItem;
        private IMovableItem carriedItem;

        Vector2 originalAnimOffset = Vector2.Zero;

        private AnimationStateMachine Animations;

        public Ladder Ladder = null;

        private List<IGameObject> overlappingEnemies = new List<IGameObject>(5);

        private bool isWallSliding = false;
        private bool isSliding
        {
            get => slideDirection == Direction.EAST || slideDirection == Direction.WEST;
        }
        private Direction slideDirection = default;

        private Direction jumpDirection = default;

        public Vector2 LastSpawnPoint = default;

        private Vector2 autoMovementSpeed = Vector2.Zero;

        private bool levelEndReached = false;

        private Random random;

        private float horizFrictBackup;

        public bool MovementButtonDown = false;

        public bool LevelEndReached
        {
            get
            {
                return levelEndReached;
            }

            set
            {
                if (value)
                {
                    SetupAutoMovement();
                }
                else
                {
                    DisableAutoMovement();
                }
                levelEndReached = value;
            }
        }

        public bool ReadyForNextLevel { get => Ladder != null; }

        public bool OnIce
        {
            set
            {
                if (value)
                {
                    HorizontalFriction = 0.92f;
                    MovementSpeed /= 5;
                }
                else
                {
                    HorizontalFriction = Config.HORIZONTAL_FRICTION;
                    MovementSpeed = Config.CHARACTER_SPEED;
                }
            }
            get
            {
                return HorizontalFriction != Config.HORIZONTAL_FRICTION;
            }
        }

        private Fan fan;

        UserInputController UserInput;

        public Hero(AbstractScene scene, Vector2 position) : base(scene.LayerManager.EntityLayer, null, position)
        {

            //DEBUG_SHOW_PIVOT = true;
            //DEBUG_SHOW_RAYCAST = true;

            DrawPriority = 0;

            LastSpawnPoint = position;

            random = new Random();

            AddCollisionAgainst(typeof(Coin));
            AddCollisionAgainst(typeof(Box));
            AddCollisionAgainst(typeof(Spring));
            AddCollisionAgainst(typeof(Carrot));
            AddCollisionAgainst(typeof(Ghost));
            AddCollisionAgainst(typeof(IceCream));
            AddCollisionAgainst(typeof(Rock));
            AddCollisionAgainst(typeof(SpikedTurtle));
            AddCollisionAgainst(typeof(Trunk));
            AddCollisionAgainst(typeof(MovingPlatform), false);
            AddCollisionAgainst(typeof(Ladder));
            AddCollisionAgainst(typeof(IceTrigger));
            AddCollisionAgainst(typeof(NextLevelTrigger));
            AddCollisionAgainst(typeof(PopupTrigger));
            AddCollisionAgainst(typeof(RespawnPoint));
            AddCollisionAgainst(typeof(SlideWall));
            AddCollisionAgainst(typeof(Bullet));
            AddCollisionAgainst(typeof(Spikes));
            AddCollisionAgainst(typeof(IceCreamProjectile));
            AddCollisionAgainst(typeof(Saw));
            AddCollisionAgainst(typeof(Fan));
            AddCollisionAgainst(typeof(GameFinishTrophy));
            AddTag("Hero");
            CanFireTriggers = true;

            BlocksRay = true;

            AddComponent(new BoxCollisionComponent(this, 16, 25, new Vector2(-8, -24)));
            //(GetCollisionComponent() as AbstractCollisionComponent).DEBUG_DISPLAY_COLLISION = true;

            //RayEmitter = new Ray2DEmitter(this);

            SetupAnimations();

            SetupController();

            CurrentFaceDirection = Direction.EAST;

            fist = new Fist(scene, this, new Vector2(20, -10));
#if DEBUG_SHOW_RAYCAST
            if (DEBUG_SHOW_RAYCAST)
            {
                /*if (RayBlockerLines == null)
                {
                    RayBlockerLines = new List<(Vector2 start, Vector2 end)>();
                }
                SetRayBlockers();

                Line l = new Line(null, GetRayBlockerLines()[0].Item1, GetRayBlockerLines()[0].Item2, Color.Blue);
                l.SetParent(this, new Vector2(-Config.GRID / 2, 0));
                l = new Line(null, GetRayBlockerLines()[1].Item1, GetRayBlockerLines()[1].Item2, Color.Blue);
                l.SetParent(this, new Vector2(0, -Config.GRID / 2 - 15));*/
            }
#endif
        }

        private void SetupAnimations()
        {
            Animations = new AnimationStateMachine();
            AddComponent(Animations);
            Animations.Offset = new Vector2(0, -32);

            CollisionOffsetRight = 0.45f;
            CollisionOffsetLeft = 0.6f;
            CollisionOffsetBottom = 1f;
            CollisionOffsetTop = 1f;

            SpriteSheetAnimation hurtRight = new SpriteSheetAnimation(this, Assets.GetTexture("HeroHurt"), 24)
            {
                Looping = false
            };
            Animations.RegisterAnimation("HurtRight", hurtRight, () => false);

            SpriteSheetAnimation hurtLeft = hurtRight.CopyFlipped();
            Animations.RegisterAnimation("HurtLeft", hurtLeft, () => false);

            SpriteSheetAnimation idleRight = new SpriteSheetAnimation(this, Assets.GetTexture("HeroIdle"), 24);
            bool isIdleRight() => CurrentFaceDirection == Direction.EAST && !isCarryingItem;
            Animations.RegisterAnimation("IdleRight", idleRight, isIdleRight);

            SpriteSheetAnimation idleLeft = idleRight.CopyFlipped();
            bool isIdleLeft() => CurrentFaceDirection == Direction.WEST && !isCarryingItem;
            Animations.RegisterAnimation("IdleLeft", idleLeft, isIdleLeft);

            SpriteSheetAnimation idleCarryRight = new SpriteSheetAnimation(this, Assets.GetTexture("HeroIdleWithItem"), 24)
            {
                AnimationSwitchCallback = () => { if (carriedItem != null) (carriedItem as Entity).GetComponent<AnimationStateMachine>().Offset = originalAnimOffset; },
                EveryFrameAction = (frame) =>
                {
                    if (carriedItem == null) return;
                    Entity e = carriedItem as Entity;
                    Vector2 offset = e.GetComponent<AnimationStateMachine>().Offset;
                    float unit = 0.5f;
                    if (frame == 2 || frame == 3 || frame == 8 || frame == 14 || frame == 15 || frame == 20)
                    {
                        offset.Y += unit;
                    }
                    else if (frame == 6 || frame == 18)
                    {
                        offset.Y -= unit;
                    }
                    else if (frame == 7 || frame == 19)
                    {
                        offset.Y -= 2 * unit;
                    }
                    e.GetComponent<AnimationStateMachine>().Offset = offset;
                }
            };
            bool isIdleCarryRight() => CurrentFaceDirection == Direction.EAST && isCarryingItem;
            Animations.RegisterAnimation("IdleCarryRight", idleCarryRight, isIdleCarryRight);

            SpriteSheetAnimation idleCarryLeft = idleCarryRight.CopyFlipped();
            bool isIdleCarryLeft() => CurrentFaceDirection == Direction.WEST && isCarryingItem;
            Animations.RegisterAnimation("IdleCarryLeft", idleCarryLeft, isIdleCarryLeft);

            SpriteSheetAnimation runningRight = new SpriteSheetAnimation(this, Assets.GetTexture("HeroRun"), 40);
            runningRight.EveryFrameAction = (frame) =>
            {
                if (frame == 1 || frame == 6)
                {
                    AudioEngine.Play("FastFootstepsSound");
                }
            };

            bool isRunningRight() => MovementButtonDown && Transform.VelocityX > 0.1 && !Scene.GridCollisionChecker.HasBlockingColliderAt(this, Direction.EAST) && !isCarryingItem;
            Animations.RegisterAnimation("RunningRight", runningRight, isRunningRight, 1);

            SpriteSheetAnimation runningLeft = runningRight.CopyFlipped();
            bool isRunningLeft() => MovementButtonDown && Transform.VelocityX < -0.1 && !Scene.GridCollisionChecker.HasBlockingColliderAt(this, Direction.WEST) && !isCarryingItem;
            Animations.RegisterAnimation("RunningLeft", runningLeft, isRunningLeft, 1);

            SpriteSheetAnimation walkingLeft = new SpriteSheetAnimation(this, Assets.GetTexture("HeroRun"), 12, SpriteEffects.FlipHorizontally);
            /*walkingLeft.StartedCallback = () =>
            {
                AudioEngine.Play("SlowFootstepsSound");
            };

            walkingLeft.StoppedCallback = () =>
            {
                AudioEngine.Stop("SlowFootstepsSound");
            };*/
            /*bool isWalkingLeft() => Transform.VelocityX > -0.5f && Transform.VelocityX < -0.01 && !Scene.GridCollisionChecker.HasBlockingColliderAt(this, Direction.WEST) && !isCarryingItem;
            Animations.RegisterAnimation("WalkingLeft", walkingLeft, isWalkingLeft, 1);

            SpriteSheetAnimation walkingRight = walkingLeft.CopyFlipped();
            bool isWalkingRight() => Transform.VelocityX > 0.01 && Transform.VelocityX < 0.5f && !Scene.GridCollisionChecker.HasBlockingColliderAt(this, Direction.EAST) && !isCarryingItem;
            Animations.RegisterAnimation("WalkingRight", walkingRight, isWalkingRight, 1);

            Animations.AddFrameTransition("RunningRight", "WalkingRight");
            Animations.AddFrameTransition("RunningLeft", "WalkingLeft");*/

            SpriteSheetAnimation runningCarryRight = new SpriteSheetAnimation(this, Assets.GetTexture("HeroRunWithItem"), 24)
            {
                AnimationSwitchCallback = () => { if (carriedItem != null) (carriedItem as Entity).GetComponent<AnimationStateMachine>().Offset = originalAnimOffset; },
                EveryFrameAction = (frame) =>
                {
                    if (carriedItem == null) return;
                    Entity e = carriedItem as Entity;
                    Vector2 offset = e.GetComponent<AnimationStateMachine>().Offset;
                    float unit = 3;
                    if (frame == 3 || frame == 8)
                    {
                        offset.Y += unit;
                    }
                    else if (frame == 4 || frame == 9)
                    {
                        offset.Y -= unit;
                    }
                    e.GetComponent<AnimationStateMachine>().Offset = offset;
                },
            };

            runningCarryRight.EveryFrameAction += (frame) =>
            {
                if (frame == 1 || frame == 6)
                {
                    AudioEngine.Play("FastFootstepsSound");
                }
            };

            bool isRunningCarryRight() => Transform.VelocityX >= 0.1f && !Scene.GridCollisionChecker.HasBlockingColliderAt(this, Direction.EAST) && isCarryingItem;
            Animations.RegisterAnimation("RunningCarryRight", runningCarryRight, isRunningCarryRight, 1);

            SpriteSheetAnimation runningCarryLeft = runningCarryRight.CopyFlipped();
            bool isRunningCarryLeft() => Transform.VelocityX <= -0.1f && !Scene.GridCollisionChecker.HasBlockingColliderAt(this, Direction.WEST) && isCarryingItem;
            Animations.RegisterAnimation("RunningCarryLeft", runningCarryLeft, isRunningCarryLeft, 1);

            /*SpriteSheetAnimation walkingCarryLeft = new SpriteSheetAnimation(this, Assets.GetTexture("HeroRunWithItem"), 12, SpriteEffects.FlipHorizontally);
            walkingCarryLeft.StartedCallback = () =>
            {
                AudioEngine.Play("SlowFootstepsSound");
            };

            walkingCarryLeft.StoppedCallback = () =>
            {
                AudioEngine.Stop("SlowFootstepsSound");
            };*/
            /*bool isCarryWalkingLeft() => Transform.VelocityX > -0.1f && Transform.VelocityX < 0 && !Scene.GridCollisionChecker.HasBlockingColliderAt(this, Direction.WEST) && isCarryingItem;
            Animations.RegisterAnimation("WalkingCarryLeft", walkingCarryLeft, isCarryWalkingLeft, 1);

            SpriteSheetAnimation walkingCarryRight = walkingCarryLeft.CopyFlipped();
            bool isCarryWalkingRight() => Transform.VelocityX > 0 && Transform.VelocityX < 0.1f && !Scene.GridCollisionChecker.HasBlockingColliderAt(this, Direction.EAST) && isCarryingItem;
            Animations.RegisterAnimation("WalkingCarryRight", walkingCarryRight, isCarryWalkingRight, 1);

            Animations.AddFrameTransition("RunningCarryRight", "WalkingCarryRight");
            Animations.AddFrameTransition("RunningCarryLeft", "WalkingCarryLeft");*/

            SpriteSheetAnimation jumpRight = new SpriteSheetAnimation(this, Assets.GetTexture("HeroJump"), 24)
            {
                Looping = false
            };
            bool isJumpingRight() => FallSpeed > 0f && CurrentFaceDirection == Direction.EAST && !isCarryingItem;
            Animations.RegisterAnimation("JumpingRight", jumpRight, isJumpingRight, 2);

            SpriteSheetAnimation jumpLeft = jumpRight.CopyFlipped();
            bool isJumpingLeft() => FallSpeed > 0f && CurrentFaceDirection == Direction.WEST && !isCarryingItem;
            Animations.RegisterAnimation("JumpingLeft", jumpLeft, isJumpingLeft, 2);

            Animations.AddFrameTransition("JumpingRight", "JumpingLeft");

            SpriteSheetAnimation jumpCarryRight = new SpriteSheetAnimation(this, Assets.GetTexture("HeroJumpWithItem"), 24)
            {
                Looping = false
            };
            bool isCarryJumpingRight() => FallSpeed > 0f && CurrentFaceDirection == Direction.EAST && isCarryingItem;
            Animations.RegisterAnimation("CarryJumpingRight", jumpCarryRight, isCarryJumpingRight, 2);

            SpriteSheetAnimation jumpCarryLeft = jumpCarryRight.CopyFlipped();
            bool isCarryJumpingLeft() => FallSpeed > 0f && CurrentFaceDirection == Direction.WEST && isCarryingItem;
            Animations.RegisterAnimation("JumpingCarryLeft", jumpCarryLeft, isCarryJumpingLeft, 2);

            Animations.AddFrameTransition("CarryJumpingRight", "JumpingCarryLeft");

            SpriteSheetAnimation wallSlideRight = new SpriteSheetAnimation(this, Assets.GetTexture("HeroWallSlide"), 12, SpriteEffects.FlipHorizontally, 64);
            bool isWallSlidingRight() => isWallSliding && CurrentFaceDirection == Direction.EAST;
            Animations.RegisterAnimation("WallSlideRight", wallSlideRight, isWallSlidingRight, 6);
            wallSlideRight.Offset += new Vector2(6, 0);

            SpriteSheetAnimation wallSlideLeft = wallSlideRight.CopyFlipped();
            wallSlideRight.Offset += new Vector2(-12, 0);
            bool isWallSlidingLeft() => isWallSliding && CurrentFaceDirection == Direction.WEST;
            Animations.RegisterAnimation("WallSlideLeft", wallSlideLeft, isWallSlidingLeft, 6);

            SpriteSheetAnimation doubleJumpRight = new SpriteSheetAnimation(this, Assets.GetTexture("HeroDoubleJump"), 12)
            {
                StartFrame = 12,
                EndFrame = 16
            };
            bool isDoubleJumpingRight() => !IsOnGround && Transform.VelocityY != 0 && doubleJumping && CurrentFaceDirection == Direction.EAST;
            Animations.RegisterAnimation("DoubleJumpingRight", doubleJumpRight, isDoubleJumpingRight, 3);

            SpriteSheetAnimation doubleJumpLeft = doubleJumpRight.CopyFlipped();
            bool isDoubleJumpingLeft() => !IsOnGround && Transform.VelocityY != 0 && doubleJumping && CurrentFaceDirection == Direction.WEST;
            Animations.RegisterAnimation("DoubleJumpingLeft", doubleJumpLeft, isDoubleJumpingLeft, 3);

            Animations.AddFrameTransition("DoubleJumpingRight", "DoubleJumpingLeft");

            SpriteSheetAnimation climb = new SpriteSheetAnimation(this, Assets.GetTexture("HeroClimb"), 40);

            climb.EveryFrameAction = (frame) =>
            {
                if (frame == 1 || frame == 7)
                {
                    AudioEngine.Play("FastFootstepsSound");
                }
            };

            void climbResetAction()
            {
                if (Ladder != null)
                {
                    MovementSpeed = climbSpeed;
                    HorizontalFriction = 0.1f;
                    VerticalFriction = 0.1f;
                }
                else
                {
                    LeaveLadder();
                }
            }

            void setSpeed(int frame)
            {
                if (frame == 1 || frame == 7)
                {
                    Transform.Velocity = Vector2.Zero;
                    MovementSpeed = 0f;
                    Timer.TriggerAfter(50, climbResetAction, true);
                }
            }

            climb.AddFrameAction(1, setSpeed);
            climb.AddFrameAction(7, setSpeed);

            bool isClimbing() => Ladder != null && !IsOnGround;
            //bool isHangingOnLadder() => (Math.Abs(Transform.VelocityX) <= 0.1f && Math.Abs(Transform.VelocityY) <= 0.1f);
            bool isHangingOnLadder() => Transform.Velocity == Vector2.Zero;
            climb.AnimationPauseCondition = isHangingOnLadder;

            Animations.RegisterAnimation("ClimbingLadder", climb, isClimbing, 6);

            /*SpriteSheetAnimation slowClimb = new SpriteSheetAnimation(this, Assets.GetTexture("HeroClimb"), 15);
            bool isSlowClimbing() =>  !IsOnGround && Ladder != null && ((Math.Abs(Transform.VelocityX) > 0.01f && Math.Abs(Transform.VelocityX) < 0.5) || (Math.Abs(Transform.VelocityY) > 0.01f && Math.Abs(Transform.VelocityY) < 0.5));
            slowClimb.EveryFrameAction = setSpeed;
            Animations.RegisterAnimation("SlowClimbingLadder", slowClimb, isSlowClimbing, 7);

            Animations.AddFrameTransition("ClimbingLadder", "SlowClimbingLadder");*/

            SpriteSheetAnimation fallingRight = new SpriteSheetAnimation(this, Assets.GetTexture("HeroJump"), 24)
            {
                StartFrame = 9,
                EndFrame = 11
            };
            bool isFallingRight() => fan != null || (HasGravity && Transform.VelocityY > 0.1 && CurrentFaceDirection == Direction.EAST && !isCarryingItem);
            Animations.RegisterAnimation("FallingRight", fallingRight, isFallingRight, 5);

            SpriteSheetAnimation fallingLeft = fallingRight.CopyFlipped();
            bool isFallingLeft() => fan != null || (HasGravity && Transform.VelocityY > 0.1 && CurrentFaceDirection == Direction.WEST && !isCarryingItem);
            Animations.RegisterAnimation("FallingLeft", fallingLeft, isFallingLeft, 5);

            Animations.AddFrameTransition("FallingRight", "FallingLeft");

            SpriteSheetAnimation fallingCarryRight = new SpriteSheetAnimation(this, Assets.GetTexture("HeroJumpWithItem"), 24)
            {
                StartFrame = 9,
                EndFrame = 11
            };
            bool isCarryFallingRight() => HasGravity && Transform.VelocityY > 0.1 && CurrentFaceDirection == Direction.EAST && isCarryingItem;
            Animations.RegisterAnimation("CarryFallingRight", fallingCarryRight, isCarryFallingRight, 5);

            SpriteSheetAnimation fallingCarryLeft = fallingCarryRight.CopyFlipped();
            bool isCarryFallingLeft() => HasGravity && Transform.VelocityY > 0.1 && CurrentFaceDirection == Direction.WEST && isCarryingItem;
            Animations.RegisterAnimation("CarryFallingLeft", fallingCarryLeft, isCarryFallingLeft, 5);

            Animations.AddFrameTransition("CarryFallingRight", "CarryFallingLeft");

            SpriteSheetAnimation attackRight = new SpriteSheetAnimation(this, Assets.GetTexture("HeroAttack"), 48)
            {
                Looping = false
            };
            Animations.RegisterAnimation("AttackRight", attackRight, () => false, 8);

            SpriteSheetAnimation attackLeft = attackRight.CopyFlipped();
            Animations.RegisterAnimation("AttackLeft", attackLeft, () => false, 8);

            SpriteSheetAnimation pickupRight = new SpriteSheetAnimation(this, Assets.GetTexture("HeroPickup"), 24)
            {
                Looping = false,
                StartedCallback = () => UserInput.ControlsDisabled = true,
                StoppedCallback = () => UserInput.ControlsDisabled = false
            };
            pickupRight.AddFrameAction(15, (frame) => { 
                carriedItem.Lift(this, new Vector2(0, -20));
                AudioEngine.Play("BoxPickup");
            });
            Animations.RegisterAnimation("PickupRight", pickupRight, () => false);

            SpriteSheetAnimation pickupLeft = pickupRight.CopyFlipped();
            Animations.RegisterAnimation("PickupLeft", pickupLeft, () => false);

            SpriteSheetAnimation slideRight = new SpriteSheetAnimation(this, Assets.GetTexture("HeroSlide"), 24)
            {
                Looping = false
            };
            slideRight.StoppedCallback = () =>
            {
                slideDirection = default;
                canAttack = true;
                HorizontalFriction = horizFrictBackup;
            };
            slideRight.AnimationSwitchCallback = () =>
            {
                slideDirection = default;
                canAttack = true;
                HorizontalFriction = horizFrictBackup;
            };
            bool isSlidingRight() => IsOnGround && isSliding && CurrentFaceDirection == Direction.EAST;
            Animations.RegisterAnimation("SlideRight", slideRight, isSlidingRight, 7);

            SpriteSheetAnimation slideLeft = slideRight.CopyFlipped();
            bool isSlidingLeft() => IsOnGround && isSliding && CurrentFaceDirection == Direction.WEST;
            Animations.RegisterAnimation("SlideLeft", slideLeft, isSlidingLeft, 7);

            Animations.AddFrameTransition("SlideRight", "SlideLeft");

        }

        private void SetupController()
        {
            UserInput = new UserInputController();
            AddComponent(UserInput);

            UserInput.RegisterKeyPressAction(Keys.R, () =>
            {
                ResetPosition(new Vector2(12 * Config.GRID, 12 * Config.GRID));
            }, true);

            UserInput.RegisterKeyPressAction(Keys.Right, MoveRight);

            UserInput.RegisterKeyPressAction(Keys.Left, MoveLeft);

            UserInput.RegisterKeyPressAction(Keys.Up, Jump, true);

            UserInput.RegisterKeyPressAction(Keys.Space, AttackOrThrow, true);

            UserInput.RegisterKeyPressAction(Keys.RightControl, Slide, true);

            UserInput.RegisterKeyPressAction(Keys.LeftControl, Slide, true);

            UserInput.RegisterKeyPressAction(Keys.Down, ClimbDownOrDescend);

            UserInput.RegisterKeyReleaseAction(Keys.Down, ClimbDescendRelease);

            UserInput.RegisterKeyPressAction(Keys.Up, ClimbUpOnLadder);

            UserInput.RegisterKeyPressAction(Keys.LeftShift, InteractWithItem, true);

            UserInput.RegisterKeyPressAction(Keys.RightShift,InteractWithItem, true);

            UserInput.RegisterKeyReleaseAction(Keys.Left, () => MovementButtonDown = false);

            UserInput.RegisterKeyReleaseAction(Keys.Right, () => MovementButtonDown = false);

            UserInput.RegisterMouseActions(
                () =>
                {
                    Timer.Repeat(300, (elapsedTime) =>
                    {
                        foreach (Camera camera in Scene.Cameras)
                        {
                            camera.Zoom += 0.002f * elapsedTime;
                        }
                    });
                },
                () =>
                {
                    Timer.Repeat(300, (elapsedTime) =>
                    {
                        foreach (Camera camera in Scene.Cameras)
                        {
                            camera.Zoom -= 0.002f * elapsedTime;
                        }
                    });
                }
            );
        }

        public void MoveLeft()
        {
            Move(Direction.WEST);
        }

        public void MoveRight()
        {
            Move(Direction.EAST);
        }

        private void Move(Direction direction)
        {
            int mul = 1;
            if (direction == Direction.WEST)
            {
                mul = -1;
            }
            if (slideDirection != direction)
            {
                Transform.VelocityX += mul * MovementSpeed * (float)Globals.GameTime.ElapsedGameTime.TotalSeconds * Config.TIME_OFFSET;
            }
            CurrentFaceDirection = direction;
            fist.ChangeDirection();
            MovementButtonDown = true;
        }

        public void Jump()
        {
            if (Ladder != null || (!canJump && !canDoubleJump))
            {
                return;
            }
            if (canJump)
            {
                if (!isCarryingItem)
                {
                    canDoubleJump = true;
                }
                canJump = false;
            }
            else
            {
                if (lastJump < JUMP_RATE)
                {
                    return;
                }
                lastJump = 0f;
                canDoubleJump = false;
                doubleJumping = true;
            }

            Transform.VelocityY -= Config.JUMP_FORCE + jumpModifier.Y;
            Transform.VelocityX += jumpModifier.X;
            if (jumpModifier.X < 0)
            {
                CurrentFaceDirection = Direction.WEST;
            }
            else if (jumpModifier.X > 0)
            {
                CurrentFaceDirection = Direction.EAST;
            }
            jumpModifier = Vector2.Zero;
            FallSpeed = (float)Globals.GameTime.TotalGameTime.TotalSeconds;
            AudioEngine.Play("JumpSound");
        }

        public void ClimbUpOnLadder()
        {
            if (Ladder == null)
            {
                return;
            }

            Transform.VelocityY -= MovementSpeed * (float)Globals.GameTime.ElapsedGameTime.TotalSeconds * Config.TIME_OFFSET;
        }

        private bool descended = false;
        public void ClimbDownOrDescend()
        {
            if (Ladder == null)
            {
                if (HasGravity && !descended)
                {
                    StaticCollider collider = Scene.GridCollisionChecker.GetColliderAt(GridUtil.GetBelowGrid(this));
                    if (collider != null && collider.HasTag("Platform") && collider.BlocksMovement)
                    {
                        collider.BlocksMovement = false;
                        Timer.TriggerAfter(500, () => collider.BlocksMovement = true);
                        descended = true;
                    }
                }
                return;
            } 
            else 
            {
                if (IsOnGround)
                {
                    LeaveLadder();
                    return;
                }
                Transform.VelocityY += MovementSpeed * (float)Globals.GameTime.ElapsedGameTime.TotalSeconds * Config.TIME_OFFSET;
            }
        }

        public void ClimbDescendRelease()
        {
            descended = false;
        }

        public void InteractWithItem()
        {
            if (isCarryingItem && carriedItem != null)
            {
                DropCurrentItem();
            }
            else
            {
                PickupItem();
            }
        }

        public void AttackOrThrow()
        {
            if (isCarryingItem)
            {
                (carriedItem as Entity).GetComponent<AnimationStateMachine>().Offset = originalAnimOffset;
                Vector2 force;
                if (CurrentFaceDirection == Direction.WEST)
                {
                    force = new Vector2(-2.5f, 0);
                }
                else
                {
                    force = new Vector2(2.5f, 0);
                }
                ThrowCurrentItem(force);
                return;
            }
            Attack();
        }

        public void Slide()
        {
            if (isSliding || !IsOnGround || isCarryingItem || isWallSliding || Ladder != null)
            {
                return;
            }
            slideDirection = CurrentFaceDirection;
            canAttack = false;
            horizFrictBackup = HorizontalFriction;
            HorizontalFriction = 0.9f;
            if (CurrentFaceDirection == Direction.EAST)
            {
                Transform.VelocityX = SLIDE_FORCE;
            }
            else
            {
                Transform.VelocityX = -SLIDE_FORCE;
            }
        }

        private void PickupItem()
        {

            if (overlappingItem == null || carriedItem != null)
            {
                return;
            }
            Entity e = (overlappingItem as Entity);
            /*if (e.Transform.X < Transform.X && CurrentFaceDirection != Direction.WEST)
            {
                return;
            }
            if (e.Transform.X > Transform.X && CurrentFaceDirection != Direction.EAST)
            {
                return;
            }*/
            if (CurrentFaceDirection == Direction.WEST)
            {
                Animations.PlayAnimation("PickupLeft");
            }
            else
            {
                Animations.PlayAnimation("PickupRight");
            }
            carriedItem = overlappingItem;
            originalAnimOffset = e.GetComponent<AnimationStateMachine>().Offset;
            isCarryingItem = true;
        }

        private void ThrowCurrentItem(Vector2 force)
        {
            if (isCarryingItem && carriedItem != null)
            {
                carriedItem.Throw(this, force);
                isCarryingItem = false;
                carriedItem = null;
            }
        }

        private void DropCurrentItem()
        {
            ThrowCurrentItem(new Vector2(0, -0.5f));
        }

        private void Attack()
        {
            if (canAttack)
            {
                fist.Attack();
            }
        }

        public override void FixedUpdate()
        {

            if (LevelEndReached && Ladder != null)
            {
                Transform.Velocity += autoMovementSpeed;
            }
            else if (fan != null)
            {
                canJump = false;
                canDoubleJump = false;
                FallSpeed = 0f;
                float posDiff = fan.Transform.Y - Transform.Y;
                float updraft = MathHelper.Lerp(0.5f, 0, (float)(posDiff / fan.ForceFieldHeight));
                if (random.Next(0, 11) % 2 == 0)
                {
                    updraft = 0;
                }
                Transform.VelocityY -= updraft;
            }
            else
            {
                if (Ladder != null && !IsOnGround)
                {
                    SetupLadderMovement();
                }
                else if (Ladder != null)
                {
                    ExitLadderMovement();
                }

                if (!isSliding && overlappingEnemies.Count > 0 && !Timer.IsSet("Invincible"))
                {
                    if (overlappingEnemies[0] is Spikes)
                    {
                        Hit(overlappingEnemies[0], true, new Vector2(0, -1.5f));
                    }
                    else
                    {
                        Hit(overlappingEnemies[0]);
                    }

                }

                if (Transform.Y > 2000)
                {
                    Respawn();
                }
            }

            base.FixedUpdate();
        }

        public override void Update()
        {
            if (Ladder == null)
            {
                if (HasGravity && IsOnGround)
                {
                    FallSpeed = 0;
                    if (Transform.VelocityY == 0)
                    {
                        canJump = true;
                        canDoubleJump = false;
                    }
                    doubleJumping = false;
                }

                if (FallSpeed > 0)
                {
                    lastJump += Globals.ElapsedTime;
                }
                else
                {
                    doubleJumping = false;
                }
            }

            base.Update();
        }

        protected override void SetRayBlockers()
        {
            RayBlockerLines.Clear();
            RayBlockerLines.Add((new Vector2(Transform.X - Config.GRID / 2, Transform.Y - 10), new Vector2(Transform.X + Config.GRID / 2, Transform.Y - 10)));
            RayBlockerLines.Add((new Vector2(Transform.X, Transform.Y - Config.GRID / 2 - 10), new Vector2(Transform.X, Transform.Y + Config.GRID / 2 - 10)));
        }

        public void EnterLadder(Ladder ladder)
        {
            Ladder = ladder;
            Transform.VelocityY = 0;
            SetupLadderMovement();
        }

        private void SetupLadderMovement()
        {
            canJump = false;
            canDoubleJump = true;
            MovementSpeed = climbSpeed;
            HasGravity = false;
        }

        private void ExitLadderMovement()
        {
            if (Ladder != null && Ladder.Transform.Position.Y > Transform.Y && Transform.VelocityY < 0)
            {
                Transform.VelocityY -= Config.JUMP_FORCE;
            }
            FallSpeed = 0;

            HasGravity = true;
            MovementSpeed = Config.CHARACTER_SPEED;
            HorizontalFriction = Config.HORIZONTAL_FRICTION;
            VerticalFriction = Config.VERTICAL_FRICTION;
        }

        public void LeaveLadder()
        {
            ExitLadderMovement();
            Ladder = null;
        }

        private void Hit(IGameObject otherCollider, bool usePositionCheck = true, Vector2 forceRightFacing = default)
        {
            if (Timer.IsSet("Invincible"))
            {
                return;
            }
            FallSpeed = 0;
            AudioEngine.Play("HeroHurtSound");
            CancelVelocities();
            DropCurrentItem();
            UserInput.ControlsDisabled = true;
            Timer.SetTimer("Invincible", (float)TimeSpan.FromSeconds(1).TotalMilliseconds, true);
            Timer.TriggerAfter((float)TimeSpan.FromSeconds(0.5).TotalMilliseconds, () => UserInput.ControlsDisabled = false);
            Timer.TriggerAfter((float)TimeSpan.FromSeconds(0.5).TotalMilliseconds, () => canAttack = true);

            if (CurrentFaceDirection == Direction.WEST)
            {
                Animations.PlayAnimation("HurtLeft");
            }
            else if (CurrentFaceDirection == Direction.EAST)
            {
                Animations.PlayAnimation("HurtRight");
            }

            if (forceRightFacing == default)
            {
                forceRightFacing = new Vector2(1, -1);
            }

            if (usePositionCheck)
            {
                if (otherCollider.Transform.X > Transform.X)
                {
                    forceRightFacing.X *= -1;
                }
                Transform.Velocity += forceRightFacing;
            }
            else
            {
                if (((otherCollider as PhysicalEntity).Transform.VelocityX == 0))
                {
                    forceRightFacing.X = 0;
                }
                else if (((otherCollider as PhysicalEntity).Transform.VelocityX < 0))
                {
                    forceRightFacing.X *= -1;
                }
                Transform.Velocity += forceRightFacing;
            }

        }

        public override void OnCollisionStart(IGameObject otherCollider)
        {
            if (otherCollider.HasTag("Enemy"))
            {
                overlappingEnemies.Add(otherCollider);

                if (!isSliding)
                {
                    if (otherCollider is SpikedTurtle && (otherCollider as SpikedTurtle).SpikesOut)
                    {
                        Hit(otherCollider);
                        return;
                    }
                    float angle = MathUtil.DegreeFromVectors(Transform.Position, otherCollider.Transform.Position);
                    if (Transform.VelocityY > 0 && angle <= 155 && angle >= 25 && !Timer.IsSet("Invincible"))
                    {
                        Transform.VelocityY = 0;
                        Bump(new Vector2(0, -0.5f));
                        FallSpeed = 0;
                        (otherCollider as AbstractEnemy).Hit(Direction.NORTH);
                        Timer.SetTimer("Invincible", (float)TimeSpan.FromSeconds(0.5).TotalMilliseconds, true);
                        canJump = false;
                        canDoubleJump = true;
                    }
                    else
                    {
                        Hit(otherCollider);
                    }
                    if (otherCollider is Carrot)
                    {
                        (otherCollider as Carrot).OverlapsWithHero = true;
                    }
                }
            }
            else if (otherCollider is Bullet)
            {
                if (!isSliding)
                {
                    Hit(otherCollider, false);
                }
            }
            else if (otherCollider is Coin)
            {
                AudioEngine.Play("CoinPickupSound");
                (otherCollider as Coin).Die();
                PlatformerGame.CoinCount++;
            }
            else if (otherCollider is Box && (otherCollider as Box).Transform.Velocity == Vector2.Zero && Transform.Y < otherCollider.Transform.Y)
            {
                AudioEngine.Play("BoxBounceSound");
                Transform.VelocityY = 0;
                Bump(new Vector2(0, -0.5f));
                FallSpeed = 0;
                (otherCollider as Box).Hit(Direction.CENTER);
            }
            else if (otherCollider is Spikes)
            {
                Direction spikeDirection = (otherCollider as Spikes).Direction;
                if (spikeDirection != Direction.SOUTH || (spikeDirection == Direction.SOUTH && !isSliding))
                {
                    Hit(otherCollider, true, new Vector2(0, -1.5f));
                }
                overlappingEnemies.Add(otherCollider);
            }
            else if (otherCollider is Spring)
            {
                if (!isSliding)
                {
                    ((Spring)otherCollider).Bounce();
                    Transform.VelocityY = 0;
                    Bump(new Vector2(0, -2f));
                    canJump = false;
                    canDoubleJump = false;
                    FallSpeed = 0;
                }

            }
            else if (otherCollider is IMovableItem)
            {
                overlappingItem = otherCollider as IMovableItem;
            }
            else if (otherCollider is SlideWall && !IsOnGround)
            {
                if (Timer.IsSet("IsAttacking") || isCarryingItem)
                {
                    return;
                }
                isWallSliding = true;
                if (Transform.VelocityY < 0)
                {
                    Transform.VelocityY = 0;
                }
                if (GravityValue == Config.GRAVITY_FORCE)
                {
                    GravityValue /= 4;
                    canAttack = false;
                }
                canDoubleJump = true;
                if (otherCollider.Transform.X < Transform.X)
                {
                    jumpModifier = new Vector2(1, 0);
                }
                else if (otherCollider.Transform.X > Transform.X)
                {
                    jumpModifier = new Vector2(-1, 0);
                }
            }
            else if (otherCollider is IceCreamProjectile)
            {
                Hit(otherCollider, true);
                (otherCollider as IceCreamProjectile).DestroyBullet();
            }
            else if (otherCollider is Saw)
            {
                Hit(otherCollider, true);
            }
            else if (otherCollider is Fan)
            {
                LeaveFanArea();
                Hit(otherCollider, true);
            }
            else if (otherCollider is GameFinishTrophy)
            {
                Scene.Finish();
            }
            base.OnCollisionStart(otherCollider);
        }

        public override void OnCollisionEnd(IGameObject otherCollider)
        {
            if (otherCollider is IMovableItem)
            {
                overlappingItem = null;
            }
            else if (otherCollider is SlideWall)
            {
                isWallSliding = false;
                GravityValue = Config.GRAVITY_FORCE;
                jumpModifier = Vector2.Zero;
                canAttack = true;
            }
            /*else if (otherCollider.HasTag("Platform") && !(otherCollider as StaticCollider).BlocksMovement)
            {
                (otherCollider as StaticCollider).BlocksMovement = true;
            }*/
            else if (otherCollider.HasTag("Enemy"))
            {
                if (otherCollider is Carrot)
                {
                    (otherCollider as Carrot).OverlapsWithHero = false;
                }

                overlappingEnemies.Remove(otherCollider);
            }
            else if (otherCollider is Spikes)
            {
                overlappingEnemies.Remove(otherCollider);
            }
            base.OnCollisionEnd(otherCollider);
        }

        private void Respawn()
        {
            ResetPosition(LastSpawnPoint);
        }

        public override void Destroy()
        {
            base.Destroy();
        }

        private void SetupAutoMovement()
        {
            if (Ladder == null)
            {
                //return;
            }
            UserInput.ControlsDisabled = true;
            autoMovementSpeed = new Vector2(0, -0.3f);
        }

        private void DisableAutoMovement()
        {
            UserInput.ControlsDisabled = false;
            autoMovementSpeed = Vector2.Zero;
        }

        public void EnterFanArea(Fan fan)
        {
            this.fan = fan;
            VerticalFriction = 0;
            canJump = false;
            canDoubleJump = false;
            doubleJumping = false;
        }

        public void LeaveFanArea()
        {
            fan = null;
            VerticalFriction = Config.VERTICAL_FRICTION;
        }

    }
}
