using ForestPlatformerExample.Source.Enemies;
using ForestPlatformerExample.Source.Entities.Enemies;
using ForestPlatformerExample.Source.Entities.Interfaces;
using ForestPlatformerExample.Source.Entities.Items;
using ForestPlatformerExample.Source.Environment;
using ForestPlatformerExample.Source.Items;
using ForestPlatformerExample.Source.Weapons;
using GameEngine2D;
using GameEngine2D.Engine.Source.Entities;
using GameEngine2D.Engine.Source.Entities.Abstract;
using GameEngine2D.Engine.Source.Entities.Animations;
using GameEngine2D.Engine.Source.Entities.Controller;
using GameEngine2D.Engine.Source.Graphics.Primitives;
using GameEngine2D.Engine.Source.Level.Collision;
using GameEngine2D.Engine.Source.Physics.Collision;
using GameEngine2D.Engine.Source.Physics.Interface;
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

        public bool OnLadder = false;

        private List<IGameObject> overlappingEnemies = new List<IGameObject>(5);

        public Hero(Vector2 position, SpriteFont font = null) : base(LayerManager.Instance.EntityLayer, null, position, font)
        {

            //DEBUG_SHOW_PIVOT = true;
            //DEBUG_SHOW_RAYCAST = true;

            AddCollisionAgainst("Interactive");
            AddCollisionAgainst("Enemy");
            AddCollisionAgainst("Mountable", false);

            CanFireTriggers = true;

            BlocksRay = true;

            //AddComponent(new CircleCollisionComponent(this, 10, new Vector2(0, -10)));
            AddComponent(new BoxCollisionComponent(this, 16, 24, new Vector2(-8, -24)));
            //AddComponent(new BoxCollisionComponent(this, 16, 24, Vector2.Zero));
            (GetCollisionComponent() as AbstractCollisionComponent).DEBUG_DISPLAY_COLLISION = true;

            //ColliderOnGrid = true;

            //RayEmitter = new Ray2DEmitter(this);

            SetupAnimations();

            SetupController();

            CurrentFaceDirection = Direction.EAST;

            fist = new Fist(this, new Vector2(20, -10));

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

        private void SetupAnimations()
        {
            Animations = new AnimationStateMachine();
            AddComponent(Animations);
            //Animations.Offset = new Vector2(3, -20);
            Animations.Offset = new Vector2(0, -32);

            CollisionOffsetRight = 0.5f;
            CollisionOffsetLeft = 0.6f;
            CollisionOffsetBottom = 1f;
            CollisionOffsetTop = 0.5f;

            SpriteSheetAnimation hurtRight = new SpriteSheetAnimation(this, "ForestAssets/Characters/Hero/main-character@hurt-sheet", 24);
            hurtRight.Looping = false;
            Animations.RegisterAnimation("HurtRight", hurtRight, () => false);

            SpriteSheetAnimation hurtLeft = hurtRight.CopyFlipped();
            Animations.RegisterAnimation("HurtLeft", hurtLeft, () => false);

            SpriteSheetAnimation idleRight = new SpriteSheetAnimation(this, "ForestAssets/Characters/Hero/main-character@idle-sheet", 24);
            Func<bool> isIdleRight = () => CurrentFaceDirection == Direction.EAST && !isCarryingItem;
            Animations.RegisterAnimation("IdleRight", idleRight, isIdleRight);

            SpriteSheetAnimation idleLeft = idleRight.CopyFlipped();
            Func<bool> isIdleLeft = () => CurrentFaceDirection == Direction.WEST && !isCarryingItem;
            Animations.RegisterAnimation("IdleLeft", idleLeft, isIdleLeft);

            SpriteSheetAnimation idleCarryRight = new SpriteSheetAnimation(this, "ForestAssets/Characters/Hero/main-character@idle-with-item-sheet", 24);
            idleCarryRight.AnimationSwitchCallback = () => { if (carriedItem != null) (carriedItem as Entity).GetComponent<AnimationStateMachine>().Offset = originalAnimOffset; };
            idleCarryRight.EveryFrameAction = (frame) => {
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
            };
            Func<bool> isIdleCarryRight = () => CurrentFaceDirection == Direction.EAST && isCarryingItem;
            Animations.RegisterAnimation("IdleCarryRight", idleCarryRight, isIdleCarryRight);

            SpriteSheetAnimation idleCarryLeft = idleCarryRight.CopyFlipped();
            Func<bool> isIdleCarryLeft = () => CurrentFaceDirection == Direction.WEST && isCarryingItem;
            Animations.RegisterAnimation("IdleCarryLeft", idleCarryLeft, isIdleCarryLeft);

            SpriteSheetAnimation runningRight = new SpriteSheetAnimation(this, "ForestAssets/Characters/Hero/main-character@run-sheet", 24);
            Func<bool> isRunningRight = () => VelocityX > 0.5f && !GridCollisionChecker.Instance.HasBlockingColliderAt(Transform.GridCoordinates, Direction.EAST) && !isCarryingItem;
            Animations.RegisterAnimation("RunningRight", runningRight, isRunningRight, 1);

            SpriteSheetAnimation runningLeft = runningRight.CopyFlipped();
            Func<bool> isRunningLeft = () => VelocityX < -0.5f && !GridCollisionChecker.Instance.HasBlockingColliderAt(Transform.GridCoordinates, Direction.WEST) && !isCarryingItem;
            Animations.RegisterAnimation("RunningLeft", runningLeft, isRunningLeft, 1);

            SpriteSheetAnimation walkingLeft = new SpriteSheetAnimation(this, "ForestAssets/Characters/Hero/main-character@run-sheet", 12, SpriteEffects.FlipHorizontally);
            Func<bool> isWalkingLeft = () => VelocityX > -0.5f && VelocityX < -0.1 && !GridCollisionChecker.Instance.HasBlockingColliderAt(Transform.GridCoordinates, Direction.WEST);
            Animations.RegisterAnimation("WalkingLeft", walkingLeft, isWalkingLeft, 1);

            SpriteSheetAnimation walkingRight = walkingLeft.CopyFlipped();
            Func<bool> isWalkingRight = () => VelocityX > 0.1 && VelocityX < 0.5f && !GridCollisionChecker.Instance.HasBlockingColliderAt(Transform.GridCoordinates, Direction.EAST);
            Animations.RegisterAnimation("WalkingRight", walkingRight, isWalkingRight, 1);

            Animations.AddFrameTransition("RunningRight", "WalkingRight");
            Animations.AddFrameTransition("RunningLeft", "WalkingLeft");

            SpriteSheetAnimation runningCarryRight = new SpriteSheetAnimation(this, "ForestAssets/Characters/Hero/main-character@run-with-item-sheet", 24);
            Func<bool> isRunningCarryRight = () => VelocityX > 0.5f && !GridCollisionChecker.Instance.HasBlockingColliderAt(Transform.GridCoordinates, Direction.EAST) && isCarryingItem;
            runningCarryRight.AnimationSwitchCallback = () => { if (carriedItem != null) (carriedItem as Entity).GetComponent<AnimationStateMachine>().Offset = originalAnimOffset; };
            runningCarryRight.EveryFrameAction = (frame) => {
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
            };
            Animations.RegisterAnimation("RunningCarryRight", runningCarryRight, isRunningCarryRight, 1);

            SpriteSheetAnimation runningCarryLeft = runningCarryRight.CopyFlipped();
            Func<bool> isRunningCarryLeft = () => VelocityX < -0.5f && !GridCollisionChecker.Instance.HasBlockingColliderAt(Transform.GridCoordinates, Direction.WEST) && isCarryingItem;
            Animations.RegisterAnimation("RunningCarryLeft", runningCarryLeft, isRunningCarryLeft, 1);

            SpriteSheetAnimation walkingCarryLeft = new SpriteSheetAnimation(this, "ForestAssets/Characters/Hero/main-character@run-with-item-sheet", 12, SpriteEffects.FlipHorizontally);
            Func<bool> isCarryWalkingLeft = () => VelocityX > -0.5f && VelocityX < -0.1 && !GridCollisionChecker.Instance.HasBlockingColliderAt(Transform.GridCoordinates, Direction.WEST) && isCarryingItem;
            Animations.RegisterAnimation("WalkingCarryLeft", walkingCarryLeft, isCarryWalkingLeft, 1);

            SpriteSheetAnimation walkingCarryRight = walkingCarryLeft.CopyFlipped();
            Func<bool> isCarryWalkingRight = () => VelocityX > 0.1 && VelocityX < 0.5f && !GridCollisionChecker.Instance.HasBlockingColliderAt(Transform.GridCoordinates, Direction.EAST) && isCarryingItem;
            Animations.RegisterAnimation("WalkingCarryRight", walkingCarryRight, isCarryWalkingRight, 1);

            Animations.AddFrameTransition("RunningCarryRight", "WalkingCarryRight");
            Animations.AddFrameTransition("RunningCarryLeft", "WalkingCarryLeft");

            SpriteSheetAnimation jumpRight = new SpriteSheetAnimation(this, "ForestAssets/Characters/Hero/main-character@jump-sheet", 24);
            jumpRight.Looping = false;
            Func<bool> isJumpingRight = () => FallSpeed > 0f && CurrentFaceDirection == Direction.EAST && !isCarryingItem;
            Animations.RegisterAnimation("JumpingRight", jumpRight, isJumpingRight, 2);

            SpriteSheetAnimation jumpLeft = jumpRight.CopyFlipped();
            Func<bool> isJumpingLeft = () => FallSpeed > 0f && CurrentFaceDirection == Direction.WEST && !isCarryingItem;
            Animations.RegisterAnimation("JumpingLeft", jumpLeft, isJumpingLeft, 2);

            Animations.AddFrameTransition("JumpingRight", "JumpingLeft");

            SpriteSheetAnimation jumpCarryRight = new SpriteSheetAnimation(this, "ForestAssets/Characters/Hero/main-character@jump-with-item-sheet", 24);
            jumpCarryRight.Looping = false;
            Func<bool> isCarryJumpingRight = () => FallSpeed > 0f && CurrentFaceDirection == Direction.EAST && isCarryingItem;
            Animations.RegisterAnimation("CarryJumpingRight", jumpCarryRight, isCarryJumpingRight, 2);

            SpriteSheetAnimation jumpCarryLeft = jumpCarryRight.CopyFlipped();
            Func<bool> isCarryJumpingLeft = () => FallSpeed > 0f && CurrentFaceDirection == Direction.WEST && isCarryingItem;
            Animations.RegisterAnimation("JumpingCarryLeft", jumpCarryLeft, isCarryJumpingLeft, 2);

            Animations.AddFrameTransition("CarryJumpingRight", "JumpingCarryLeft");

            SpriteSheetAnimation wallSlideRight = new SpriteSheetAnimation(this, "ForestAssets/Characters/Hero/main-character@wall-slide-sheet", 12,SpriteEffects.FlipHorizontally, 64);
            Func<bool> isWallSlidingRight = () => jumpModifier != Vector2.Zero && CurrentFaceDirection == Direction.EAST;
            Animations.RegisterAnimation("WallSlideRight", wallSlideRight, isWallSlidingRight, 6);

            SpriteSheetAnimation wallSlideLeft = wallSlideRight.CopyFlipped();
            Func<bool> isWallSlidingLeft = () => jumpModifier != Vector2.Zero && CurrentFaceDirection == Direction.WEST;
            Animations.RegisterAnimation("WallSlideLeft", wallSlideLeft, isWallSlidingLeft, 6);

            SpriteSheetAnimation doubleJumpRight = new SpriteSheetAnimation(this, "ForestAssets/Characters/Hero/main-character@double-jump-sheet", 12);
            doubleJumpRight.StartFrame = 12;
            doubleJumpRight.EndFrame = 16;
            Func<bool> isDoubleJumpingRight = () => doubleJumping && CurrentFaceDirection == Direction.EAST;
            Animations.RegisterAnimation("DoubleJumpingRight", doubleJumpRight, isDoubleJumpingRight, 3);

            SpriteSheetAnimation doubleJumpLeft = doubleJumpRight.CopyFlipped();
            Func<bool> isDoubleJumpingLeft = () => doubleJumping && CurrentFaceDirection == Direction.WEST;
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

            Func<bool> isClimbing = () => OnLadder;
            Func<bool> isHangingOnLadder = () => (Math.Abs(VelocityX) <= 0.1f && Math.Abs(VelocityY) <= 0.1f);
            climb.AnimationPauseCondition = isHangingOnLadder;
            Animations.RegisterAnimation("ClimbingLadder", climb, isClimbing, 6);

            SpriteSheetAnimation slowClimb = new SpriteSheetAnimation(this, "ForestAssets/Characters/Hero/main-character@climb-sheet", 15);
            Func<bool> isSlowClimbing = () => OnLadder && ((Math.Abs(VelocityX) > 0.01f && Math.Abs(VelocityX) < 0.5) || (Math.Abs(VelocityY) > 0.01f && Math.Abs(VelocityY) < 0.5));
            slowClimb.EveryFrameAction = setSpeed;
            Animations.RegisterAnimation("SlowClimbingLadder", slowClimb, isSlowClimbing, 7);

            Animations.AddFrameTransition("ClimbingLadder", "SlowClimbingLadder");

            SpriteSheetAnimation fallingRight = new SpriteSheetAnimation(this, "ForestAssets/Characters/Hero/main-character@jump-sheet", 24);
            fallingRight.StartFrame = 9;
            fallingRight.EndFrame = 11;
            Func<bool> isFallingRight = () => HasGravity && VelocityY > 0.1 && CurrentFaceDirection == Direction.EAST && !isCarryingItem;
            Animations.RegisterAnimation("FallingRight", fallingRight, isFallingRight, 5);

            SpriteSheetAnimation fallingLeft = fallingRight.CopyFlipped();
            Func<bool> isFallingLeft = () => HasGravity && VelocityY > 0.1 && CurrentFaceDirection == Direction.WEST && !isCarryingItem;
            Animations.RegisterAnimation("FallingLeft", fallingLeft, isFallingLeft, 5);

            Animations.AddFrameTransition("FallingRight", "FallingLeft");

            SpriteSheetAnimation fallingCarryRight = new SpriteSheetAnimation(this, "ForestAssets/Characters/Hero/main-character@jump-with-item-sheet", 24);
            fallingCarryRight.StartFrame = 9;
            fallingCarryRight.EndFrame = 11;
            Func<bool> isCarryFallingRight = () => HasGravity && VelocityY > 0.1 && CurrentFaceDirection == Direction.EAST && isCarryingItem;
            Animations.RegisterAnimation("CarryFallingRight", fallingCarryRight, isCarryFallingRight, 5);

            SpriteSheetAnimation fallingCarryLeft = fallingCarryRight.CopyFlipped();
            Func<bool> isCarryFallingLeft = () => HasGravity && VelocityY > 0.1 && CurrentFaceDirection == Direction.WEST && isCarryingItem;
            Animations.RegisterAnimation("CarryFallingLeft", fallingCarryLeft, isCarryFallingLeft, 5);

            Animations.AddFrameTransition("CarryFallingRight", "CarryFallingLeft");

            SpriteSheetAnimation attackRight = new SpriteSheetAnimation(this, "ForestAssets/Characters/Hero/main-character@attack-sheet", 48);
            attackRight.Looping = false;
            Animations.RegisterAnimation("AttackRight", attackRight, () => false, 8);

            SpriteSheetAnimation attackLeft = attackRight.CopyFlipped();
            Animations.RegisterAnimation("AttackLeft", attackLeft, () => false, 8);

            SpriteSheetAnimation pickupRight = new SpriteSheetAnimation(this, "ForestAssets/Characters/Hero/main-character@pick-up-sheet", 24);
            pickupRight.Looping = false;
            pickupRight.StartedCallback = () => UserInput.ControlsDisabled = true;
            pickupRight.StoppedCallback = () => UserInput.ControlsDisabled = false;
            pickupRight.AddFrameAction(15, (frame) => carriedItem.Lift(this, new Vector2(0, -20)));
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
                    VelocityX += GetVelocity(thumbStickPosition.X, MovementSpeed) * elapsedTime;
                    if (VelocityX > 0.1)
                    {
                        CurrentFaceDirection = Direction.EAST;
                    }
                } else if (thumbStickPosition.X == 0)
                {
                    VelocityX += MovementSpeed * elapsedTime;
                    CurrentFaceDirection = Direction.EAST;
                }
                fist.ChangeDirection();
                //CurrentFaceDirection = Direction.RIGHT;
            });

            UserInput.RegisterKeyPressAction(Keys.Left, Buttons.LeftThumbstickLeft, (Vector2 thumbStickPosition) => {
                if (thumbStickPosition.X < -0)
                {
                    VelocityX += GetVelocity(thumbStickPosition.X, MovementSpeed) * elapsedTime;
                    if (VelocityX < -0.1)
                    {
                        CurrentFaceDirection = Direction.WEST;
                    }
                } else if (thumbStickPosition.X == 0)
                {
                    VelocityX -= MovementSpeed * elapsedTime;
                    CurrentFaceDirection = Direction.WEST;
                }
                fist.ChangeDirection();
                //CurrentFaceDirection = Direction.LEFT;
            });

            UserInput.RegisterKeyPressAction(Keys.Up, Buttons.A, (Vector2 thumbStickPosition) => {
                if (OnLadder || (!canJump && !canDoubleJump))
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
                FallSpeed = (float)GameTime.TotalGameTime.TotalSeconds;

            }, true);

            UserInput.RegisterKeyPressAction(Keys.Space, Buttons.X, (Vector2 thumbStickPosition) => {
                if (isCarryingItem)
                {
                    (carriedItem as Entity).GetComponent<AnimationStateMachine>().Offset = originalAnimOffset;
                    Vector2 force;
                    if (CurrentFaceDirection == Direction.WEST)
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
                Attack();
            }, true);

            UserInput.RegisterKeyPressAction(Keys.Down, Buttons.LeftThumbstickDown, (Vector2 thumbStickPosition) => {
                if (HasGravity)
                {
                    StaticCollider collider = GridCollisionChecker.Instance.GetColliderAt(GridUtil.GetBelowGrid(Transform.GridCoordinates));
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
                if (IsOnGround)
                {
                    LeaveLadder();
                    return;
                }
                if (thumbStickPosition.Y != 0)
                {
                    VelocityY -= GetVelocity(thumbStickPosition.Y, MovementSpeed) * elapsedTime;
                } 
                else
                {
                    VelocityY += MovementSpeed * elapsedTime;
                }
                //CurrentFaceDirection = GridDirection.DOWN;
            });

            UserInput.RegisterKeyPressAction(Keys.Up, Buttons.LeftThumbstickUp, (Vector2 thumbStickPosition) => {
                if (!HasGravity)
                {
                    return;
                }

                if (thumbStickPosition.Y != 0)
                {
                    VelocityY -= GetVelocity(thumbStickPosition.Y, MovementSpeed) * elapsedTime;
                }
                else
                {
                    VelocityY -= MovementSpeed * elapsedTime;
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

        public override void FixedUpdate(GameTime gameTime)
        {
            if (overlappingEnemies.Count > 0 && !Timer.IsSet("Invincible"))
            {
                Hit(overlappingEnemies[0]);
            }
            base.FixedUpdate(gameTime);
        }

        public override void Update(GameTime gameTime)
        {

            if (OnLadder)
            {
                if (!IsOnGround && HasGravity)
                {
                    VelocityY = 0;
                    MovementSpeed = climbSpeed;
                    HasGravity = false;
                }
            } 
            else
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
                    lastJump += gameTime.ElapsedGameTime.TotalSeconds;
                }
                else
                {
                    doubleJumping = false;
                }
            }
            
            base.Update(gameTime);
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

        public void LeaveLadder()
        {
            FallSpeed = 0;
            HasGravity = true;
            MovementSpeed = Config.CHARACTER_SPEED;
            if (VelocityY < -0.5)
            {
                VelocityY -= Config.JUMP_FORCE / 2;
            }
        }

        private void Hit(IGameObject otherCollider)
        {
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
            if (otherCollider.Transform.X < Transform.X)
            {
                Velocity += new Vector2(2, -2);
            }
            else if (otherCollider.Transform.X > Transform.X)
            {
                Velocity += new Vector2(-2, -2);
            }
        }

        public override void OnCollisionStart(IGameObject otherCollider)
        {
            if (otherCollider is Carrot)
            {

                overlappingEnemies.Add(otherCollider);

                float angle = MathUtil.DegreeFromVectors(Transform.Position, otherCollider.Transform.Position);
                if (angle <= 155 && angle >= 25 && !Timer.IsSet("Invincible"))
                {
                    Bump(new Vector2(0, -5));
                    FallSpeed = 0;
                    (otherCollider as Carrot).Hit(Direction.NORTH);
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

                    Hit(otherCollider);

                }
            }
            else if (otherCollider is Coin)
            {
                (otherCollider as Coin).Destroy();
                ForestPlatformer.CoinCount++;
            }
            else if (otherCollider is Box && VelocityY > 0 && (otherCollider as Box).Velocity == Vector2.Zero && Transform.Y < otherCollider.Transform.Y)
            {
                Bump(new Vector2(0, -5));
                FallSpeed = 0;
                (otherCollider as Box).Hit(Direction.CENTER);
            }
            else if (otherCollider is Spring)
            {
                ((Spring)otherCollider).PlayBounceAnimation();

                Bump(new Vector2(0, -15));
                canJump = false;
                canDoubleJump = true;
                FallSpeed = 0;
            }
            else if (otherCollider is IMovableItem)
            {
                overlappingItem = otherCollider as IMovableItem;
            }
            else if (otherCollider.HasTag("SlideWall") && !IsOnGround)
            {
                if (Timer.IsSet("IsAttacking") || isCarryingItem)
                {
                    return;
                }
                if (GravityValue == Config.GRAVITY_FORCE)
                {
                    GravityValue /= 4;
                    canAttack = false;
                }
                canDoubleJump = true;
                if (otherCollider.Transform.X > Transform.X)
                {
                    jumpModifier = new Vector2(5, 0);
                }
                else if (otherCollider.Transform.X < Transform.X)
                {
                    jumpModifier = new Vector2(-5, 0);
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
            else if (otherCollider.HasTag("SlideWall"))
            {
                GravityValue = Config.GRAVITY_FORCE;
                jumpModifier = Vector2.Zero;
                canAttack = true;
            }
            else if (otherCollider.HasTag("Platform") && !(otherCollider as StaticCollider).BlocksMovement)
            {
                (otherCollider as StaticCollider).BlocksMovement = true;
            }
            else if (otherCollider is Carrot)
            {
                overlappingEnemies.Remove(otherCollider);
            }
            base.OnCollisionEnd(otherCollider);
        }
    }
}
