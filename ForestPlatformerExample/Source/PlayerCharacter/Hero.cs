using ForestPlatformerExample.Source.Enemies;
using ForestPlatformerExample.Source.Entities.Enemies;
using ForestPlatformerExample.Source.Entities.Interfaces;
using ForestPlatformerExample.Source.Entities.Items;
using ForestPlatformerExample.Source.Environment;
using ForestPlatformerExample.Source.Items;
using ForestPlatformerExample.Source.Weapons;
using MonolithEngine;
using MonolithEngine.Engine.Source.Entities;
using MonolithEngine.Engine.Source.Entities.Abstract;
using MonolithEngine.Engine.Source.Entities.Animations;
using MonolithEngine.Engine.Source.Entities.Controller;
using MonolithEngine.Engine.Source.Global;
using MonolithEngine.Engine.Source.Graphics.Primitives;
using MonolithEngine.Engine.Source.Level.Collision;
using MonolithEngine.Engine.Source.Physics.Collision;
using MonolithEngine.Engine.Source.Physics.Interface;
using MonolithEngine.Engine.Source.Physics.Raycast;
using MonolithEngine.Engine.Source.Util;
using MonolithEngine.Entities;
using MonolithEngine.Global;
using MonolithEngine.Source.Entities;
using MonolithEngine.Source.Util;
using MonolithEngine.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;
using MonolithEngine.Engine.Source.Scene;
using MonolithEngine.Engine.Source.Asset;
using ForestPlatformerExample.Source.Entities.Enemies.Trunk;
using MonolithEngine.Engine.Source.Audio;
using ForestPlatformerExample.Source.Entities.Enemies.SpikedTurtle;

namespace ForestPlatformerExample.Source.PlayerCharacter
{
    class Hero : PhysicalEntity
    {

        private readonly float JUMP_RATE = 0.1f;
        private readonly float SLIDE_FORCE = 1f;
        private readonly bool DECREASED_AIR_MOBILITY = false;

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
        private bool isSliding {
            get => slideDirection == Direction.EAST || slideDirection == Direction.WEST;
        }
        private Direction slideDirection = default;

        private Direction jumpDirection = default;

        public Vector2 LastSpawnPoint = default;

        private Vector2 autoMovementSpeed = Vector2.Zero;

        private bool levelEndReached = false;

        public bool LevelEndReached  { 
            get {
                return levelEndReached;
            }

            set {
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
                } else
                {
                    HorizontalFriction = Config.HORIZONTAL_FRICTION;
                    MovementSpeed = Config.CHARACTER_SPEED;
                }
            }
        }

        public Hero(AbstractScene scene, Vector2 position) : base(scene.LayerManager.EntityLayer, null, position)
        {

            //DEBUG_SHOW_PIVOT = true;
            //DEBUG_SHOW_RAYCAST = true;

            DrawPriority = 0;

            LastSpawnPoint = position;

            AddCollisionAgainst("Interactive");
            AddCollisionAgainst("Enemy");
            AddCollisionAgainst("Mountable", false);
            AddCollisionAgainst("Environment");
            AddCollisionAgainst("Projectile");
            AddCollisionAgainst("Box");
            AddCollisionAgainst("Spikes");
            AddTag("Hero");
            CanFireTriggers = true;

            BlocksRay = true;

            //AddComponent(new CircleCollisionComponent(this, 10, new Vector2(0, -10)));
            AddComponent(new BoxCollisionComponent(this, 16, 25, new Vector2(-8, -24)));
            //AddComponent(new BoxCollisionComponent(this, 16, 24, Vector2.Zero));
            //(GetCollisionComponent() as AbstractCollisionComponent).DEBUG_DISPLAY_COLLISION = true;

            //ColliderOnGrid = true;

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

            /*SetSprite(SpriteUtil.CreateRectangle(16, Color.Blue));
            DrawOffset = new Vector2(-8, -16);
            CollisionOffsetBottom = 1f;*/
        }

        private void SetupAnimations()
        {
            Animations = new AnimationStateMachine();
            AddComponent(Animations);
            //Animations.Offset = new Vector2(3, -20);
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
            runningRight.StartedCallback = () =>
            {
                AudioEngine.Play("FastFootstepsSound");
            };

            runningRight.StoppedCallback = () =>
            {
                AudioEngine.Stop("FastFootstepsSound");
            };

            bool isRunningRight() => UserInput.IsKeyPressed(Keys.Right) && !Scene.GridCollisionChecker.HasBlockingColliderAt(Transform.GridCoordinates, Direction.EAST) && !isCarryingItem;
            Animations.RegisterAnimation("RunningRight", runningRight, isRunningRight, 1);

            SpriteSheetAnimation runningLeft = runningRight.CopyFlipped();
            bool isRunningLeft() => UserInput.IsKeyPressed(Keys.Left) && !Scene.GridCollisionChecker.HasBlockingColliderAt(Transform.GridCoordinates, Direction.WEST) && !isCarryingItem;
            Animations.RegisterAnimation("RunningLeft", runningLeft, isRunningLeft, 1);

            SpriteSheetAnimation walkingLeft = new SpriteSheetAnimation(this, Assets.GetTexture("HeroRun"), 12, SpriteEffects.FlipHorizontally);
            walkingLeft.StartedCallback = () =>
            {
                AudioEngine.Play("SlowFootstepsSound");
            };

            walkingLeft.StoppedCallback = () =>
            {
                AudioEngine.Stop("SlowFootstepsSound");
            };
            bool isWalkingLeft() => UserInput.IsKeyPressed(Keys.Right) && VelocityX > -0.5f && VelocityX < -0.01 && !Scene.GridCollisionChecker.HasBlockingColliderAt(Transform.GridCoordinates, Direction.WEST) && !isCarryingItem;
            Animations.RegisterAnimation("WalkingLeft", walkingLeft, isWalkingLeft, 1);

            SpriteSheetAnimation walkingRight = walkingLeft.CopyFlipped();
            bool isWalkingRight() => UserInput.IsKeyPressed(Keys.Left) && VelocityX > 0.01 && VelocityX < 0.5f && !Scene.GridCollisionChecker.HasBlockingColliderAt(Transform.GridCoordinates, Direction.EAST) && !isCarryingItem;
            Animations.RegisterAnimation("WalkingRight", walkingRight, isWalkingRight, 1);

            Animations.AddFrameTransition("RunningRight", "WalkingRight");
            Animations.AddFrameTransition("RunningLeft", "WalkingLeft");

            SpriteSheetAnimation runningCarryRight = new SpriteSheetAnimation(this, Assets.GetTexture("HeroRunWithItem"), 24)
            {
                StartedCallback = () =>
                {
                    AudioEngine.Play("FastFootstepsSound");
                },

                StoppedCallback = () =>
                {
                    AudioEngine.Stop("FastFootstepsSound");
                },
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

            bool isRunningCarryRight() => VelocityX >= 0.1f && !Scene.GridCollisionChecker.HasBlockingColliderAt(Transform.GridCoordinates, Direction.EAST) && isCarryingItem;
            Animations.RegisterAnimation("RunningCarryRight", runningCarryRight, isRunningCarryRight, 1);

            SpriteSheetAnimation runningCarryLeft = runningCarryRight.CopyFlipped();
            bool isRunningCarryLeft() => VelocityX <= -0.1f && !Scene.GridCollisionChecker.HasBlockingColliderAt(Transform.GridCoordinates, Direction.WEST) && isCarryingItem;
            Animations.RegisterAnimation("RunningCarryLeft", runningCarryLeft, isRunningCarryLeft, 1);

            SpriteSheetAnimation walkingCarryLeft = new SpriteSheetAnimation(this, Assets.GetTexture("HeroRunWithItem"), 12, SpriteEffects.FlipHorizontally);
            walkingCarryLeft.StartedCallback = () =>
            {
                AudioEngine.Play("SlowFootstepsSound");
            };

            walkingCarryLeft.StoppedCallback = () =>
            {
                AudioEngine.Stop("SlowFootstepsSound");
            };
            bool isCarryWalkingLeft() => VelocityX > -0.1f && VelocityX < 0 && !Scene.GridCollisionChecker.HasBlockingColliderAt(Transform.GridCoordinates, Direction.WEST) && isCarryingItem;
            Animations.RegisterAnimation("WalkingCarryLeft", walkingCarryLeft, isCarryWalkingLeft, 1);

            SpriteSheetAnimation walkingCarryRight = walkingCarryLeft.CopyFlipped();
            bool isCarryWalkingRight() => VelocityX > 0 && VelocityX < 0.1f && !Scene.GridCollisionChecker.HasBlockingColliderAt(Transform.GridCoordinates, Direction.EAST) && isCarryingItem;
            Animations.RegisterAnimation("WalkingCarryRight", walkingCarryRight, isCarryWalkingRight, 1);

            Animations.AddFrameTransition("RunningCarryRight", "WalkingCarryRight");
            Animations.AddFrameTransition("RunningCarryLeft", "WalkingCarryLeft");

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

            SpriteSheetAnimation wallSlideRight = new SpriteSheetAnimation(this, Assets.GetTexture("HeroWallSlide"), 12,SpriteEffects.FlipHorizontally, 64);
            bool isWallSlidingRight() => isWallSliding && CurrentFaceDirection == Direction.EAST;
            Animations.RegisterAnimation("WallSlideRight", wallSlideRight, isWallSlidingRight, 6);

            SpriteSheetAnimation wallSlideLeft = wallSlideRight.CopyFlipped();
            bool isWallSlidingLeft() => isWallSliding && CurrentFaceDirection == Direction.WEST;
            Animations.RegisterAnimation("WallSlideLeft", wallSlideLeft, isWallSlidingLeft, 6);

            SpriteSheetAnimation doubleJumpRight = new SpriteSheetAnimation(this, Assets.GetTexture("HeroDoubleJump"), 12)
            {
                StartFrame = 12,
                EndFrame = 16
            };
            bool isDoubleJumpingRight() => !IsOnGround && Velocity.Y != 0 && doubleJumping && CurrentFaceDirection == Direction.EAST;
            Animations.RegisterAnimation("DoubleJumpingRight", doubleJumpRight, isDoubleJumpingRight, 3);

            SpriteSheetAnimation doubleJumpLeft = doubleJumpRight.CopyFlipped();
            bool isDoubleJumpingLeft() => !IsOnGround && Velocity.Y != 0 && doubleJumping && CurrentFaceDirection == Direction.WEST;
            Animations.RegisterAnimation("DoubleJumpingLeft", doubleJumpLeft, isDoubleJumpingLeft, 3);

            Animations.AddFrameTransition("DoubleJumpingRight", "DoubleJumpingLeft");

            SpriteSheetAnimation climb = new SpriteSheetAnimation(this, Assets.GetTexture("HeroClimb"), 40);

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
                    Velocity = Vector2.Zero;
                    MovementSpeed = 0f;
                    Timer.TriggerAfter(50, climbResetAction, true);
                }
            }

            climb.AddFrameAction(1, setSpeed);
            climb.AddFrameAction(7, setSpeed);

            bool isClimbing() => Ladder != null && !IsOnGround;
            //bool isHangingOnLadder() => (Math.Abs(VelocityX) <= 0.1f && Math.Abs(VelocityY) <= 0.1f);
            bool isHangingOnLadder() => Velocity == Vector2.Zero;
            climb.AnimationPauseCondition = isHangingOnLadder;
            Animations.RegisterAnimation("ClimbingLadder", climb, isClimbing, 6);

            /*SpriteSheetAnimation slowClimb = new SpriteSheetAnimation(this, Assets.GetTexture("HeroClimb"), 15);
            bool isSlowClimbing() =>  !IsOnGround && Ladder != null && ((Math.Abs(VelocityX) > 0.01f && Math.Abs(VelocityX) < 0.5) || (Math.Abs(VelocityY) > 0.01f && Math.Abs(VelocityY) < 0.5));
            slowClimb.EveryFrameAction = setSpeed;
            Animations.RegisterAnimation("SlowClimbingLadder", slowClimb, isSlowClimbing, 7);

            Animations.AddFrameTransition("ClimbingLadder", "SlowClimbingLadder");*/

            SpriteSheetAnimation fallingRight = new SpriteSheetAnimation(this, Assets.GetTexture("HeroJump"), 24)
            {
                StartFrame = 9,
                EndFrame = 11
            };
            bool isFallingRight() => HasGravity && VelocityY > 0.1 && CurrentFaceDirection == Direction.EAST && !isCarryingItem;
            Animations.RegisterAnimation("FallingRight", fallingRight, isFallingRight, 5);

            SpriteSheetAnimation fallingLeft = fallingRight.CopyFlipped();
            bool isFallingLeft() => HasGravity && VelocityY > 0.1 && CurrentFaceDirection == Direction.WEST && !isCarryingItem;
            Animations.RegisterAnimation("FallingLeft", fallingLeft, isFallingLeft, 5);

            Animations.AddFrameTransition("FallingRight", "FallingLeft");

            SpriteSheetAnimation fallingCarryRight = new SpriteSheetAnimation(this, Assets.GetTexture("HeroJumpWithItem"), 24)
            {
                StartFrame = 9,
                EndFrame = 11
            };
            bool isCarryFallingRight() => HasGravity && VelocityY > 0.1 && CurrentFaceDirection == Direction.EAST && isCarryingItem;
            Animations.RegisterAnimation("CarryFallingRight", fallingCarryRight, isCarryFallingRight, 5);

            SpriteSheetAnimation fallingCarryLeft = fallingCarryRight.CopyFlipped();
            bool isCarryFallingLeft() => HasGravity && VelocityY > 0.1 && CurrentFaceDirection == Direction.WEST && isCarryingItem;
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
            pickupRight.AddFrameAction(15, (frame) => carriedItem.Lift(this, new Vector2(0, -20)));
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
                HorizontalFriction = Config.HORIZONTAL_FRICTION;
            };
            slideRight.AnimationSwitchCallback = () => {
                slideDirection = default;
                canAttack = true;
                HorizontalFriction = Config.HORIZONTAL_FRICTION;
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

            UserInput.RegisterKeyPressAction(Keys.R, (Vector2 thumbStickPosition) => {
                ResetPosition(new Vector2(12 * Config.GRID, 12 * Config.GRID));
            }, true);

            UserInput.RegisterKeyPressAction(Keys.Right, Buttons.LeftThumbstickRight,(Vector2 thumbStickPosition) => {
                if (thumbStickPosition.X > 0)
                {
                    if (slideDirection != Direction.EAST)
                    {
                        VelocityX += GetVelocity(thumbStickPosition.X, MovementSpeed) * Globals.FixedUpdateMultiplier * Config.TIME_OFFSET;
                    }
                    if (VelocityX > 0.1)
                    {
                        CurrentFaceDirection = Direction.EAST;
                    }
                } else if (thumbStickPosition.X == 0)
                {
                    if (slideDirection != Direction.EAST)
                    {
                        VelocityX += MovementSpeed * Globals.FixedUpdateMultiplier * Config.TIME_OFFSET;
                    }
                    CurrentFaceDirection = Direction.EAST;
                }
                fist.ChangeDirection();
                //CurrentFaceDirection = Direction.RIGHT;
            });

            UserInput.RegisterKeyReleaseAction(Keys.Right, Buttons.LeftThumbstickRight, () =>
            {
                if (DECREASED_AIR_MOBILITY && jumpDirection == Direction.EAST && MovementSpeed == Config.CHARACTER_SPEED)
                {
                    MovementSpeed /= 2;
                }
            });

            UserInput.RegisterKeyPressAction(Keys.Left, Buttons.LeftThumbstickLeft, (Vector2 thumbStickPosition) => {
                if (thumbStickPosition.X < -0)
                {
                    if (slideDirection != Direction.WEST)
                    {
                        VelocityX += GetVelocity(thumbStickPosition.X, MovementSpeed) * Globals.FixedUpdateMultiplier * Config.TIME_OFFSET;
                    }
                    if (VelocityX < -0.1)
                    {
                        CurrentFaceDirection = Direction.WEST;
                    }
                } else if (thumbStickPosition.X == 0)
                {
                    if (slideDirection != Direction.WEST)
                    {
                        VelocityX -= MovementSpeed * Globals.FixedUpdateMultiplier * Config.TIME_OFFSET;
                    }
                    CurrentFaceDirection = Direction.WEST;
                }
                fist.ChangeDirection();
                //CurrentFaceDirection = Direction.LEFT;
            });

            UserInput.RegisterKeyReleaseAction(Keys.Left, Buttons.LeftThumbstickLeft, () =>
            {
                if (DECREASED_AIR_MOBILITY && jumpDirection == Direction.WEST && MovementSpeed == Config.CHARACTER_SPEED)
                {
                    MovementSpeed /= 2;
                }
            });

            UserInput.RegisterKeyPressAction(Keys.Up, Buttons.A, (Vector2 thumbStickPosition) => {
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

                if (DECREASED_AIR_MOBILITY && Math.Abs(Velocity.X) < 0.1 && MovementSpeed == Config.CHARACTER_SPEED)
                {
                    MovementSpeed /= 2;
                }

                VelocityY -= Config.JUMP_FORCE + jumpModifier.Y;
                VelocityX += jumpModifier.X;
                if (jumpModifier.X < 0)
                {
                    CurrentFaceDirection = Direction.WEST;
                } else if (jumpModifier.X > 0)
                {
                    CurrentFaceDirection = Direction.EAST;
                }
                jumpModifier = Vector2.Zero;
                FallSpeed = (float)Globals.GameTime.TotalGameTime.TotalSeconds;
                AudioEngine.Play("JumpSound");

            }, true);

            UserInput.RegisterKeyPressAction(Keys.Space, Buttons.X, (Vector2 thumbStickPosition) => {
                if (isCarryingItem)
                {
                    (carriedItem as Entity).GetComponent<AnimationStateMachine>().Offset = originalAnimOffset;
                    Vector2 force;
                    if (CurrentFaceDirection == Direction.WEST)
                    {
                        force = new Vector2(-5, 0);
                    }
                    else
                    {
                        force = new Vector2(5, 0);
                    }
                    ThrowCurrentItem(force);
                    return;
                }
                Attack();
            }, true);

            UserInput.RegisterKeyPressAction(Keys.RightControl, Buttons.B, (Vector2 thumbStickPosition) => {
                Slide();
            }, true);

            UserInput.RegisterKeyPressAction(Keys.LeftControl, (Vector2 thumbStickPosition) => {
                Slide();
            }, true);

            UserInput.RegisterKeyPressAction(Keys.Down, Buttons.LeftThumbstickDown, (Vector2 thumbStickPosition) => {
                if (HasGravity)
                {
                    StaticCollider collider = Scene.GridCollisionChecker.GetColliderAt(GridUtil.GetBelowGrid(Transform.GridCoordinates));
                    if (collider != null && collider.HasTag("Platform") && collider.BlocksMovement) {
                        collider.BlocksMovement = false;
                        Timer.TriggerAfter(500, () => collider.BlocksMovement = true);
                    }
                }
                //CurrentFaceDirection = GridDirection.DOWN;
            }, true);

            UserInput.RegisterKeyPressAction(Keys.Down, Buttons.LeftThumbstickDown, (Vector2 thumbStickPosition) => {
                if (Ladder == null)
                {
                    return;
                }
                if (IsOnGround)
                {
                    LeaveLadder();
                    return;
                }
                if (thumbStickPosition.Y != 0)
                {
                    VelocityY -= GetVelocity(thumbStickPosition.Y, MovementSpeed) * Globals.FixedUpdateMultiplier * Config.TIME_OFFSET;
                } 
                else
                {
                    VelocityY += MovementSpeed * Globals.FixedUpdateMultiplier * Config.TIME_OFFSET;
                }
                //CurrentFaceDirection = GridDirection.DOWN;
            });

            UserInput.RegisterKeyPressAction(Keys.Up, Buttons.LeftThumbstickUp, (Vector2 thumbStickPosition) => {
                if (Ladder == null)
                {
                    return;
                }

                if (thumbStickPosition.Y != 0)
                {
                    VelocityY -= GetVelocity(thumbStickPosition.Y, MovementSpeed) * Globals.FixedUpdateMultiplier * Config.TIME_OFFSET;
                }
                else
                {
                    VelocityY -= MovementSpeed * Globals.FixedUpdateMultiplier * Config.TIME_OFFSET;
                }
                //CurrentFaceDirection = GridDirection.UP;
            });

            UserInput.RegisterKeyPressAction(Keys.LeftShift, Buttons.Y, (Vector2 thumbStickPosition) => {
                if (isCarryingItem  && carriedItem != null)
                {
                    DropCurrentItem();
                } else
                {
                    PickupItem();
                }
                
            }, true);

            UserInput.RegisterKeyPressAction(Keys.RightShift, (Vector2 thumbStickPosition) => {
                if (isCarryingItem && carriedItem != null)
                {
                    DropCurrentItem();
                }
                else
                {
                    PickupItem();
                }
            }, true);

            UserInput.RegisterMouseActions(
                () => {
                    Scene.Camera.Zoom += 0.5f;
                    /*Timer.Repeat(2000, (elapsedTime) =>
                    {
                        Scene.Camera.Zoom += 0.001f * elapsedTime;
                    });*/
                }, 
                () =>
                {
                    Scene.Camera.Zoom -= 0.5f;
                    /*Timer.Repeat(2000, (elapsedTime) =>
                    {
                        Scene.Camera.Zoom -= 0.001f * elapsedTime;
                    });*/
                });
        }

        private void Slide()
        {
            if (isSliding || !IsOnGround || isCarryingItem || isWallSliding || Ladder != null)
            {
                return;
            }
            slideDirection = CurrentFaceDirection;
            canAttack = false;
            HorizontalFriction = 0.9f;
            if (CurrentFaceDirection == Direction.EAST)
            {
                VelocityX = SLIDE_FORCE;
            }
            else
            {
                VelocityX = -SLIDE_FORCE;
            }
        }

        private void PickupItem()
        {
            
            if (overlappingItem == null || carriedItem != null)
            {
                return;
            }
            Entity e = (overlappingItem as Entity);
            if (e.Transform.X < Transform.X && CurrentFaceDirection != Direction.WEST)
            {
                return;
            }
            if (e.Transform.X > Transform.X && CurrentFaceDirection != Direction.EAST)
            {
                return;
            }
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
            ThrowCurrentItem(Vector2.Zero);
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
                Velocity += autoMovementSpeed;
            } 
            else
            {
                if (Ladder != null && !IsOnGround)
                {
                    SetupLadderMovement();
                } else if (Ladder != null)
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

        protected override void OnLeaveGround()
        {
            if (DECREASED_AIR_MOBILITY)
            {
                jumpDirection = Velocity.X < 0 ? Direction.WEST : Direction.EAST;
            }
        }

        protected override void OnLand(Vector2 velocity)
        {
            if (DECREASED_AIR_MOBILITY)
            {
                jumpDirection = default;
                MovementSpeed = Config.CHARACTER_SPEED;
            }
        }

        public override void Update()
        {
            if (Ladder == null)
            {
                if (HasGravity && IsOnGround)
                {
                    FallSpeed = 0;
                    if (VelocityY == 0)
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

        private float GetVelocity(float thumbStickPosition, float maxVelocity)
        {
            if (thumbStickPosition == 0)
            {
                return 0;
            }
            return thumbStickPosition / 1 * maxVelocity;
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
            VelocityY = 0;
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
            if (Ladder != null && Ladder.Transform.Position.Y > Transform.Y && Velocity.Y < 0)
            {
                VelocityY -= Config.JUMP_FORCE;
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
                Velocity += forceRightFacing;
            }
            else
            {
                if (((otherCollider as PhysicalEntity).Velocity.X == 0))
                {
                    forceRightFacing.X = 0;
                } 
                else if(((otherCollider as PhysicalEntity).Velocity.X < 0))
                {
                    forceRightFacing.X *= -1;
                }
                Velocity += forceRightFacing;
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
                    if (Velocity.Y > 0 && angle <= 155 && angle >= 25 && !Timer.IsSet("Invincible"))
                    {
                        VelocityY = 0;
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
            else if (otherCollider.HasTag("Projectile"))
            {
                if (!isSliding)
                {
                    Hit(otherCollider, false);
                }
            }
            else if (otherCollider is Coin)
            {
                AudioEngine.Play("CoinPickupSound");
                (otherCollider as Coin).Destroy();
                ForestPlatformerGame.CoinCount++;
            }
            else if (otherCollider is Box && (otherCollider as Box).Velocity == Vector2.Zero && Transform.Y < otherCollider.Transform.Y)
            {
                AudioEngine.Play("BoxBounceSound");
                VelocityY = 0;
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
                    VelocityY = 0;
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
                if (Velocity.Y < 0)
                {
                    VelocityY = 0;
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

    }
}
