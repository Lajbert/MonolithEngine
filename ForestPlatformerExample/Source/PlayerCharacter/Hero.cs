using ForestPlatformerExample.Source.Enemies;
using ForestPlatformerExample.Source.Entities.Interfaces;
using ForestPlatformerExample.Source.Entities.Items;
using ForestPlatformerExample.Source.Items;
using ForestPlatformerExample.Source.Weapons;
using GameEngine2D;
using GameEngine2D.Engine.Source.Entities;
using GameEngine2D.Engine.Source.Entities.Animations;
using GameEngine2D.Engine.Source.Entities.Controller;
using GameEngine2D.Engine.Source.Graphics.Primitives;
using GameEngine2D.Engine.Source.Physics.Collision;
using GameEngine2D.Engine.Source.Physics.Raycast;
using GameEngine2D.Engine.Source.Util;
using GameEngine2D.Entities;
using GameEngine2D.Global;
using GameEngine2D.Source.Entities;
using GameEngine2D.Source.Util;
using GameEngine2D.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForestPlatformerExample.Source.PlayerCharacter
{
    class Hero : PhysicalEntity
    {

        private readonly float JUMP_RATE = 0.1f;
        private static double lastJump = 0f;
        private bool doubleJumping = false;

        private bool onMovingPlatform = false;

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

        public Hero(Vector2 position, SpriteFont font = null) : base(LayerManager.Instance.EntityLayer, null, position, null, font)
        {

            //DEBUG_SHOW_PIVOT = true;
            //DEBUG_SHOW_CIRCLE_COLLIDER = true;
            //DEBUG_SHOW_RAYCAST = true;

            BlocksRay = true;

            CircleCollider = new CircleCollider(this, 10, new Vector2(0, -10));

            //ColliderOnGrid = true;

            //RayEmitter = new Ray2DEmitter(this);

            SetupAnimations();

            SetupController();

            CurrentFaceDirection = Direction.RIGHT;

            GridCollisionChecker.RestrictDirectionsForTag("Ladder", new HashSet<Direction> { Direction.UP, Direction.CENTER });

            foreach (Direction direction in new List<Direction>() { Direction.CENTER, Direction.UP, Direction.DOWN, Direction.LEFT, Direction.RIGHT })
            {
                GridCollisionCheckDirections.Add(direction);
            }

            fist = new Fist(this, new Vector2(15, -7));

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

            /*SetSprite(SpriteUtil.CreateRectangle(16, Color.Blue));
            DrawOffset = new Vector2(-8, -16);
            CollisionOffsetBottom = 1f;*/

        }

        private Texture2D red = SpriteUtil.CreateRectangle(Config.GRID, Color.Red);
        private Texture2D blue = SpriteUtil.CreateRectangle(Config.GRID, Color.Blue);
        private void SetupAnimations()
        {
            Animations = new AnimationStateMachine();
            //Animations.Offset = new Vector2(3, -20);
            Animations.Offset = new Vector2(0, -32);

            CollisionOffsetRight = 0.7f;
            CollisionOffsetLeft = 0.2f;
            CollisionOffsetBottom = 1f;
            CollisionOffsetTop = 0.5f;

            SpriteSheetAnimation hurtRight = new SpriteSheetAnimation(this, "ForestAssets/Characters/Hero/main-character@hurt-sheet", 24);
            hurtRight.Looping = false;
            Animations.RegisterAnimation("HurtRight", hurtRight, () => false);

            SpriteSheetAnimation hurtLeft = hurtRight.CopyFlipped();
            Animations.RegisterAnimation("HurtLeft", hurtLeft, () => false);

            SpriteSheetAnimation idleRight = new SpriteSheetAnimation(this, "ForestAssets/Characters/Hero/main-character@idle-sheet", 24);
            Func<bool> isIdleRight = () => CurrentFaceDirection == Direction.RIGHT && !isCarryingItem;
            Animations.RegisterAnimation("IdleRight", idleRight, isIdleRight);

            SpriteSheetAnimation idleLeft = idleRight.CopyFlipped();
            Func<bool> isIdleLeft = () => CurrentFaceDirection == Direction.LEFT && !isCarryingItem;
            Animations.RegisterAnimation("IdleLeft", idleLeft, isIdleLeft);

            SpriteSheetAnimation idleCarryRight = new SpriteSheetAnimation(this, "ForestAssets/Characters/Hero/main-character@idle-with-item-sheet", 24);
            idleCarryRight.AnimationSwitchCallback = () => { if (carriedItem != null) (carriedItem as Entity).Animations.Offset = originalAnimOffset; };
            idleCarryRight.EveryFrameAction = (frame) => {
                if (carriedItem == null) return;
                Entity e = carriedItem as Entity;
                Vector2 offset = e.Animations.Offset;
                float unit = 0.5f;
                if (frame == 3 || frame == 4 || frame == 9 || frame == 15 || frame == 16 || frame == 21)
                {
                    offset.Y += unit;
                }
                else if (frame == 7 || frame == 19)
                {
                    offset.Y -= unit;
                }
                else if (frame == 8 || frame == 20)
                {
                    offset.Y -= 2 * unit;
                }
                e.Animations.Offset = offset;
            };
            Func<bool> isIdleCarryRight = () => CurrentFaceDirection == Direction.RIGHT && isCarryingItem;
            Animations.RegisterAnimation("IdleCarryRight", idleCarryRight, isIdleCarryRight);

            SpriteSheetAnimation idleCarryLeft = idleCarryRight.CopyFlipped();
            Func<bool> isIdleCarryLeft = () => CurrentFaceDirection == Direction.LEFT && isCarryingItem;
            Animations.RegisterAnimation("IdleCarryLeft", idleCarryLeft, isIdleCarryLeft);

            SpriteSheetAnimation runningRight = new SpriteSheetAnimation(this, "ForestAssets/Characters/Hero/main-character@run-sheet", 24);
            Func<bool> isRunningRight = () => Velocity.X > 0.5f && !GridCollisionChecker.HasBlockingColliderAt(GridCoordinates, Direction.RIGHT) && !isCarryingItem;
            Animations.RegisterAnimation("RunningRight", runningRight, isRunningRight, 1);

            SpriteSheetAnimation runningLeft = runningRight.CopyFlipped();
            Func<bool> isRunningLeft = () => Velocity.X < -0.5f && !GridCollisionChecker.HasBlockingColliderAt(GridCoordinates, Direction.LEFT) && !isCarryingItem;
            Animations.RegisterAnimation("RunningLeft", runningLeft, isRunningLeft, 1);

            SpriteSheetAnimation walkingLeft = new SpriteSheetAnimation(this, "ForestAssets/Characters/Hero/main-character@run-sheet", 12, SpriteEffects.FlipHorizontally);
            Func<bool> isWalkingLeft = () => Velocity.X > -0.5f && Velocity.X < -0.1 && !GridCollisionChecker.HasBlockingColliderAt(GridCoordinates, Direction.LEFT);
            Animations.RegisterAnimation("WalkingLeft", walkingLeft, isWalkingLeft, 1);

            SpriteSheetAnimation walkingRight = walkingLeft.CopyFlipped();
            Func<bool> isWalkingRight = () => Velocity.X > 0.1 && Velocity.X < 0.5f && !GridCollisionChecker.HasBlockingColliderAt(GridCoordinates, Direction.RIGHT);
            Animations.RegisterAnimation("WalkingRight", walkingRight, isWalkingRight, 1);

            Animations.AddFrameTransition("RunningRight", "WalkingRight");
            Animations.AddFrameTransition("RunningLeft", "WalkingLeft");

            SpriteSheetAnimation runningCarryRight = new SpriteSheetAnimation(this, "ForestAssets/Characters/Hero/main-character@run-with-item-sheet", 24);
            Func<bool> isRunningCarryRight = () => Velocity.X > 0.5f && !GridCollisionChecker.HasBlockingColliderAt(GridCoordinates, Direction.RIGHT) && isCarryingItem;
            runningCarryRight.AnimationSwitchCallback = () => { if (carriedItem != null) (carriedItem as Entity).Animations.Offset = originalAnimOffset; };
            runningCarryRight.EveryFrameAction = (frame) => {
                if (carriedItem == null) return;
                Entity e = carriedItem as Entity;
                Vector2 offset = e.Animations.Offset;
                float unit = 3;
                if (frame == 4 || frame == 9)
                {
                    offset.Y += unit;
                }
                else if (frame == 5 || frame == 10)
                {
                    offset.Y -= unit;
                }
                e.Animations.Offset = offset;
            };
            Animations.RegisterAnimation("RunningCarryRight", runningCarryRight, isRunningCarryRight, 1);

            SpriteSheetAnimation runningCarryLeft = runningCarryRight.CopyFlipped();
            Func<bool> isRunningCarryLeft = () => Velocity.X < -0.5f && !GridCollisionChecker.HasBlockingColliderAt(GridCoordinates, Direction.LEFT) && isCarryingItem;
            Animations.RegisterAnimation("RunningCarryLeft", runningCarryLeft, isRunningCarryLeft, 1);

            SpriteSheetAnimation walkingCarryLeft = new SpriteSheetAnimation(this, "ForestAssets/Characters/Hero/main-character@run-with-item-sheet", 12, SpriteEffects.FlipHorizontally);
            Func<bool> isCarryWalkingLeft = () => Velocity.X > -0.5f && Velocity.X < -0.1 && !GridCollisionChecker.HasBlockingColliderAt(GridCoordinates, Direction.LEFT) && isCarryingItem;
            Animations.RegisterAnimation("WalkingCarryLeft", walkingCarryLeft, isCarryWalkingLeft, 1);

            SpriteSheetAnimation walkingCarryRight = walkingCarryLeft.CopyFlipped();
            Func<bool> isCarryWalkingRight = () => Velocity.X > 0.1 && Velocity.X < 0.5f && !GridCollisionChecker.HasBlockingColliderAt(GridCoordinates, Direction.RIGHT) && isCarryingItem;
            Animations.RegisterAnimation("WalkingCarryRight", walkingCarryRight, isCarryWalkingRight, 1);

            Animations.AddFrameTransition("RunningCarryRight", "WalkingCarryRight");
            Animations.AddFrameTransition("RunningCarryLeft", "WalkingCarryLeft");

            SpriteSheetAnimation jumpRight = new SpriteSheetAnimation(this, "ForestAssets/Characters/Hero/main-character@jump-sheet", 24);
            jumpRight.Looping = false;
            Func<bool> isJumpingRight = () => FallSpeed > 0f && CurrentFaceDirection == Direction.RIGHT && !isCarryingItem;
            Animations.RegisterAnimation("JumpingRight", jumpRight, isJumpingRight, 2);

            SpriteSheetAnimation jumpLeft = jumpRight.CopyFlipped();
            Func<bool> isJumpingLeft = () => FallSpeed > 0f && CurrentFaceDirection == Direction.LEFT && !isCarryingItem;
            Animations.RegisterAnimation("JumpingLeft", jumpLeft, isJumpingLeft, 2);

            Animations.AddFrameTransition("JumpingRight", "JumpingLeft");

            SpriteSheetAnimation jumpCarryRight = new SpriteSheetAnimation(this, "ForestAssets/Characters/Hero/main-character@jump-with-item-sheet", 24);
            jumpCarryRight.Looping = false;
            Func<bool> isCarryJumpingRight = () => FallSpeed > 0f && CurrentFaceDirection == Direction.RIGHT && isCarryingItem;
            Animations.RegisterAnimation("CarryJumpingRight", jumpCarryRight, isCarryJumpingRight, 2);

            SpriteSheetAnimation jumpCarryLeft = jumpCarryRight.CopyFlipped();
            Func<bool> isCarryJumpingLeft = () => FallSpeed > 0f && CurrentFaceDirection == Direction.LEFT && isCarryingItem;
            Animations.RegisterAnimation("JumpingCarryLeft", jumpCarryLeft, isCarryJumpingLeft, 2);

            Animations.AddFrameTransition("CarryJumpingRight", "JumpingCarryLeft");

            SpriteSheetAnimation wallSlideRight = new SpriteSheetAnimation(this, "ForestAssets/Characters/Hero/main-character@wall-slide-sheet", 12,SpriteEffects.FlipHorizontally, 64);
            Func<bool> isWallSlidingRight = () => jumpModifier != Vector2.Zero && CurrentFaceDirection == Direction.RIGHT;
            Animations.RegisterAnimation("WallSlideRight", wallSlideRight, isWallSlidingRight, 6);

            SpriteSheetAnimation wallSlideLeft = wallSlideRight.CopyFlipped();
            Func<bool> isWallSlidingLeft = () => jumpModifier != Vector2.Zero && CurrentFaceDirection == Direction.LEFT;
            Animations.RegisterAnimation("WallSlideLeft", wallSlideLeft, isWallSlidingLeft, 6);

            SpriteSheetAnimation doubleJumpRight = new SpriteSheetAnimation(this, "ForestAssets/Characters/Hero/main-character@double-jump-sheet", 12);
            doubleJumpRight.StartFrame = 12;
            doubleJumpRight.EndFrame = 16;
            Func<bool> isDoubleJumpingRight = () => doubleJumping && CurrentFaceDirection == Direction.RIGHT;
            Animations.RegisterAnimation("DoubleJumpingRight", doubleJumpRight, isDoubleJumpingRight, 3);

            SpriteSheetAnimation doubleJumpLeft = doubleJumpRight.CopyFlipped();
            Func<bool> isDoubleJumpingLeft = () => doubleJumping && CurrentFaceDirection == Direction.LEFT;
            Animations.RegisterAnimation("DoubleJumpingLeft", doubleJumpLeft, isDoubleJumpingLeft, 3);

            Animations.AddFrameTransition("DoubleJumpingRight", "DoubleJumpingLeft");

            SpriteSheetAnimation climb = new SpriteSheetAnimation(this, "ForestAssets/Characters/Hero/main-character@climb-sheet", 60);

            Action climbResetAction = () => {
                MovementSpeed = climbSpeed;
            };

            Action<int> setSpeed = frame =>
            {
                if (frame == 1 || frame == 7)
                {
                    Velocity = Vector2.Zero;
                    MovementSpeed = 0f;
                    Timer.TriggerAfter(50, climbResetAction, true);
                }
            };

            climb.AddFrameAction(1, setSpeed);
            climb.AddFrameAction(7, setSpeed);

            Func<bool> isClimbing = () => !HasGravity;
            Func<bool> isHangingOnLadder = () => (Math.Abs(Velocity.X) <= 0.1f && Math.Abs(Velocity.Y) <= 0.1f);
            climb.AnimationPauseCondition = isHangingOnLadder;
            Animations.RegisterAnimation("ClimbingLadder", climb, isClimbing, 6);

            SpriteSheetAnimation slowClimb = new SpriteSheetAnimation(this, "ForestAssets/Characters/Hero/main-character@climb-sheet", 15);
            Func<bool> isSlowClimbing = () => !HasGravity && ((Math.Abs(Velocity.X) > 0.01f && Math.Abs(Velocity.X) < 0.5) || (Math.Abs(Velocity.Y) > 0.01f && Math.Abs(Velocity.Y) < 0.5));
            slowClimb.EveryFrameAction = setSpeed;
            Animations.RegisterAnimation("SlowClimbingLadder", slowClimb, isSlowClimbing, 7);

            Animations.AddFrameTransition("ClimbingLadder", "SlowClimbingLadder");

            SpriteSheetAnimation fallingRight = new SpriteSheetAnimation(this, "ForestAssets/Characters/Hero/main-character@jump-sheet", 24);
            fallingRight.StartFrame = 9;
            fallingRight.EndFrame = 11;
            Func<bool> isFallingRight = () => HasGravity && Velocity.Y > 0.1 && CurrentFaceDirection == Direction.RIGHT && !isCarryingItem;
            Animations.RegisterAnimation("FallingRight", fallingRight, isFallingRight, 5);

            SpriteSheetAnimation fallingLeft = fallingRight.CopyFlipped();
            Func<bool> isFallingLeft = () => HasGravity && Velocity.Y > 0.1 && CurrentFaceDirection == Direction.LEFT && !isCarryingItem;
            Animations.RegisterAnimation("FallingLeft", fallingLeft, isFallingLeft, 5);

            Animations.AddFrameTransition("FallingRight", "FallingLeft");

            SpriteSheetAnimation fallingCarryRight = new SpriteSheetAnimation(this, "ForestAssets/Characters/Hero/main-character@jump-with-item-sheet", 24);
            fallingCarryRight.StartFrame = 9;
            fallingCarryRight.EndFrame = 11;
            Func<bool> isCarryFallingRight = () => HasGravity && Velocity.Y > 0.1 && CurrentFaceDirection == Direction.RIGHT && isCarryingItem;
            Animations.RegisterAnimation("CarryFallingRight", fallingCarryRight, isCarryFallingRight, 5);

            SpriteSheetAnimation fallingCarryLeft = fallingCarryRight.CopyFlipped();
            Func<bool> isCarryFallingLeft = () => HasGravity && Velocity.Y > 0.1 && CurrentFaceDirection == Direction.LEFT && isCarryingItem;
            Animations.RegisterAnimation("CarryFallingLeft", fallingCarryLeft, isCarryFallingLeft, 5);

            Animations.AddFrameTransition("CarryFallingRight", "CarryFallingLeft");

            SpriteSheetAnimation attackRight = new SpriteSheetAnimation(this, "ForestAssets/Characters/Hero/main-character@attack-sheet", 48);
            //attackRight.StartedCallback = () => Velocity.X = 0f;
            attackRight.AddFrameAction(5, (frame) => canAttack = true);
            attackRight.AddFrameAction(5, (frame) => fist.IsAttacking = false);
            //attackRight.EveryFrameAction = (frame) => HitEnemy();
            //attackRight.StoppedCallback += () => isAttacking = false;
            attackRight.Looping = false;
            Animations.RegisterAnimation("AttackRight", attackRight, () => false, 8);

            SpriteSheetAnimation attackLeft = attackRight.CopyFlipped();
            Animations.RegisterAnimation("AttackLeft", attackLeft, () => false, 8);

            SpriteSheetAnimation pickupRight = new SpriteSheetAnimation(this, "ForestAssets/Characters/Hero/main-character@pick-up-sheet", 24);
            pickupRight.Looping = false;
            pickupRight.StartedCallback = () => UserInput.ControlsDisabled = true;
            pickupRight.StoppedCallback = () => UserInput.ControlsDisabled = false;
            pickupRight.AddFrameAction(15, (frame) => carriedItem.Lift(this, new Vector2(0, -25)));
            Animations.RegisterAnimation("PickupRight", pickupRight, () => false);

            SpriteSheetAnimation pickupLeft = pickupRight.CopyFlipped();
            Animations.RegisterAnimation("PickupLeft", pickupLeft, () => false);

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
                    Velocity.X += GetVelocity(thumbStickPosition.X, MovementSpeed) * elapsedTime;
                    if (Velocity.X > 0.1)
                    {
                        CurrentFaceDirection = Direction.RIGHT;
                    }
                } else if (thumbStickPosition.X == 0)
                {
                    Velocity.X += MovementSpeed * elapsedTime;
                    CurrentFaceDirection = Direction.RIGHT;
                }
                fist.ChangeDirection();
                //CurrentFaceDirection = Direction.RIGHT;
            });

            UserInput.RegisterKeyPressAction(Keys.Left, Buttons.LeftThumbstickLeft, (Vector2 thumbStickPosition) => {
                if (thumbStickPosition.X < -0)
                {
                    Velocity.X += GetVelocity(thumbStickPosition.X, MovementSpeed) * elapsedTime;
                    if (Velocity.X < -0.1)
                    {
                        CurrentFaceDirection = Direction.LEFT;
                    }
                } else if (thumbStickPosition.X == 0)
                {
                    Velocity.X -= MovementSpeed * elapsedTime;
                    CurrentFaceDirection = Direction.LEFT;
                }
                fist.ChangeDirection();
                //CurrentFaceDirection = Direction.LEFT;
            });

            UserInput.RegisterKeyPressAction(Keys.Up, Buttons.A, (Vector2 thumbStickPosition) => {
                if (!HasGravity || (!canJump && !canDoubleJump))
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

                Velocity.Y -= Config.JUMP_FORCE + jumpModifier.Y;
                Velocity.X += jumpModifier.X;
                if (jumpModifier.X < 0)
                {
                    CurrentFaceDirection = Direction.LEFT;
                } else if (jumpModifier.X > 0)
                {
                    CurrentFaceDirection = Direction.RIGHT;
                }
                jumpModifier = Vector2.Zero;
                FallSpeed = (float)GameTime.TotalGameTime.TotalSeconds;
            }, true);

            UserInput.RegisterKeyPressAction(Keys.Space, Buttons.X, (Vector2 thumbStickPosition) => {
                if (isCarryingItem)
                {
                    (carriedItem as Entity).Animations.Offset = originalAnimOffset;
                    Vector2 force;
                    if (CurrentFaceDirection == Direction.LEFT)
                    {
                        force = new Vector2(-5, -0.5f);
                    }
                    else
                    {
                        force = new Vector2(5, -0.5f);
                    }
                    ThrowCurrentItem(force);
                    return;
                }
                if (!canAttack)
                {
                    return;
                }
                canAttack = false;
                fist.IsAttacking = true;
                if (CurrentFaceDirection == Direction.LEFT)
                {
                    Animations.PlayAnimation("AttackLeft");
                } else if (CurrentFaceDirection == Direction.RIGHT)
                {
                    Animations.PlayAnimation("AttackRight");
                }
                

            }, true);

            UserInput.RegisterKeyPressAction(Keys.Down, Buttons.LeftThumbstickDown, (Vector2 thumbStickPosition) => {
                if (HasGravity)
                {
                    Entity collider = GridCollisionChecker.GetColliderAt(GridUtil.GetBelowGrid(GridCoordinates)) as Entity;
                    if (collider != null && collider.HasTag("Platform") && collider.BlocksMovement) {
                        collider.BlocksMovement = false;
                    }
                }
                //CurrentFaceDirection = GridDirection.DOWN;
            }, true);

            UserInput.RegisterKeyPressAction(Keys.Down, Buttons.LeftThumbstickDown, (Vector2 thumbStickPosition) => {
                if (HasGravity)
                {
                    return;
                }
                if (OnGround())
                {
                    LeaveLadder();
                    return;
                }
                if (thumbStickPosition.Y != 0)
                {
                    Velocity.Y -= GetVelocity(thumbStickPosition.Y, MovementSpeed) * elapsedTime;
                } 
                else
                {
                    Velocity.Y += MovementSpeed * elapsedTime;
                }
                //CurrentFaceDirection = GridDirection.DOWN;
            });

            UserInput.RegisterKeyPressAction(Keys.Up, Buttons.LeftThumbstickUp, (Vector2 thumbStickPosition) => {
                if (HasGravity && (GetSamePositionCollider() == null || !GetSamePositionCollider().HasTag("Ladder")))
                {
                    return;
                }

                if (thumbStickPosition.Y != 0)
                {
                    Velocity.Y -= GetVelocity(thumbStickPosition.Y, MovementSpeed) * elapsedTime;
                }
                else
                {
                    Velocity.Y -= MovementSpeed * elapsedTime;
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

            UserInput.RegisterMouseActions(() => { Config.ZOOM += 0.5f; /*Globals.Camera.Recenter(); */ }, () => { Config.ZOOM -= 0.5f; /*Globals.Camera.Recenter(); */});
        }

        private void PickupItem()
        {
            
            if (overlappingItem == null || carriedItem != null)
            {
                return;
            }
            Entity e = (overlappingItem as Entity);
            if (e.X < X && CurrentFaceDirection != Direction.LEFT)
            {
                return;
            }
            if (e.X > X && CurrentFaceDirection != Direction.RIGHT)
            {
                return;
            }
            if (CurrentFaceDirection == Direction.LEFT)
            {
                Animations.PlayAnimation("PickupLeft");
            }
            else
            {
                Animations.PlayAnimation("PickupRight");
            }
            carriedItem = overlappingItem;
            originalAnimOffset = e.Animations.Offset;
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

        public override void Update(GameTime gameTime)
        {

            if (HasGravity && OnGround())
            {
                FallSpeed = 0;
                if (Velocity.Y == 0)
                {
                    canJump = true;
                    canDoubleJump = false;
                }
                doubleJumping = false;
            }

            if (FallSpeed > 0)
            {
                lastJump += gameTime.ElapsedGameTime.TotalSeconds;
            } else
            {
                doubleJumping = false;
            }
            base.Update(gameTime);
        }

        protected override void OnGridCollisionStart(Entity otherCollider, Direction direction)
        {
            if (otherCollider.HasTag("MovingPlatform"))
            {
                onMovingPlatform = true;
            }
            else if (otherCollider.HasTag("Ladder") && !OnGround())
            {
                if (HasGravity)
                {
                    Velocity.Y = 0;
                    MovementSpeed = climbSpeed;
                    HasGravity = false;
                }
            }
            else if (otherCollider.HasTag("SlideWall") && !OnGround())
            {
                if (fist.IsAttacking || isCarryingItem)
                {
                    return;
                }
                if (GravityValue == Config.GRAVITY_FORCE)
                {
                    GravityValue /= 4;
                    canAttack = false;
                }
                canDoubleJump = true;
                if (direction == Direction.LEFT)
                {
                    jumpModifier = new Vector2(5, 0);
                } else if (direction == Direction.RIGHT)
                {
                    jumpModifier = new Vector2(-5, 0);
                }
            }
        }

        private float GetVelocity(float thumbStickPosition, float maxVelocity)
        {
            if (thumbStickPosition == 0)
            {
                return 0;
            }
            return thumbStickPosition / 1 * maxVelocity;
        }

        protected override void OnGridCollisionEnd(Entity otherCollider, Direction direction)
        {
            if (otherCollider.HasTag("MovingPlatform"))
            {
                onMovingPlatform = false;
            }
            else if (otherCollider.HasTag("Platform") && !otherCollider.BlocksMovement)
            {
                otherCollider.BlocksMovement = true;
            }
            else if (otherCollider.HasTag("Ladder") && GridCollisionChecker.CollidesWithTag(this, "Ladder").Count == 0)
            {
                LeaveLadder();
            }
            else if (otherCollider.HasTag("SlideWall") && GridCollisionChecker.CollidesWithTag(this, "SlideWall").Count == 0)
            {
                GravityValue = Config.GRAVITY_FORCE;
                jumpModifier = Vector2.Zero;
                canAttack = true;
            }
        }

        protected override void OnCircleCollisionStart(Entity otherCollider, CollisionResult collisionResult)
        {
            if (otherCollider is Carrot)
            {
                float angle = MathUtil.DegreeFromVectors(Position, otherCollider.Position);
                if (angle <= 155 && angle >= 25 && !Timer.IsSet("Invincible"))
                {
                    Bump(new Vector2(0, -5));
                    FallSpeed = 0;
                    (otherCollider as Carrot).Hit(Direction.UP);
                    Timer.SetTimer("Invincible", (float)TimeSpan.FromSeconds(0.5).TotalMilliseconds, true);
                    canJump = false;
                    canDoubleJump = true;
                } 
                else
                {
                    if (Timer.IsSet("Invincible"))
                    {
                        return;
                    }
                    DropCurrentItem();
                    UserInput.ControlsDisabled = true;
                    Timer.SetTimer("Invincible", (float)TimeSpan.FromSeconds(1).TotalMilliseconds, true);
                    Timer.TriggerAfter((float)TimeSpan.FromSeconds(0.5).TotalMilliseconds, () => UserInput.ControlsDisabled = false);
                    Timer.TriggerAfter((float)TimeSpan.FromSeconds(0.5).TotalMilliseconds, () => canAttack = true);

                    if (CurrentFaceDirection == Direction.LEFT)
                    {
                        Animations.PlayAnimation("HurtLeft");
                    }
                    else if (CurrentFaceDirection == Direction.RIGHT)
                    {
                        Animations.PlayAnimation("HurtRight");
                    }
                    if (otherCollider.X < X)
                    {
                        Velocity += new Vector2(2, -2);
                    }
                    else if (otherCollider.X > X)
                    {
                        Velocity += new Vector2(-2, -2);
                    }
                }
                
            } 
            else if (otherCollider is Coin)
            {
                otherCollider.Destroy();
            }
            else if (otherCollider is Box && Velocity.Y > 0 && (otherCollider as Box).Velocity == Vector2.Zero)
            {
                Bump(new Vector2(0, -5));
                FallSpeed = 0;
                (otherCollider as Box).Hit(Direction.CENTER);
            } else if (otherCollider is IMovableItem)
            {
                overlappingItem = otherCollider as IMovableItem;
            }
            else if (otherCollider is Spring)
            {
                ((Spring)otherCollider).PlayBounceAnimation();

                Bump(new Vector2(0, -15));
                canJump = false;
                canDoubleJump = true;
                FallSpeed = 0;
            }
        }

        protected override void OnCircleCollisionEnd(Entity otherCollider)
        {
            if (otherCollider is IMovableItem)
            {
                overlappingItem = null;
            }
        }

        protected override void SetRayBlockers()
        {
            RayBlockerLines.Clear();
            RayBlockerLines.Add((new Vector2(Position.X - Config.GRID / 2, Position.Y - 10), new Vector2(Position.X + Config.GRID / 2, Position.Y - 10)));
            RayBlockerLines.Add((new Vector2(Position.X, Position.Y - Config.GRID / 2 - 10), new Vector2(Position.X, Position.Y + Config.GRID / 2 - 10)));
        }

        private void LeaveLadder()
        {
            if (!HasGravity)
            {
                FallSpeed = 0;
                HasGravity = true;
                MovementSpeed = Config.CHARACTER_SPEED;
                if (Velocity.Y < -0.5)
                {
                    Velocity.Y -= Config.JUMP_FORCE / 2;
                }
            }
        }
    }
}
