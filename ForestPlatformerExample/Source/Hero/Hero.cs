using ForestPlatformerExample.Source.Enemies;
using ForestPlatformerExample.Source.Items;
using GameEngine2D;
using GameEngine2D.Engine.Source.Entities;
using GameEngine2D.Engine.Source.Entities.Animations;
using GameEngine2D.Engine.Source.Entities.Controller;
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

namespace ForestPlatformerExample.Source.Hero
{
    class Hero : ControllableEntity
    {

        private readonly float JUMP_RATE = 0.1f;
        private static double lastJump = 0f;
        private bool doubleJumping = false;

        private bool onMovingPlatform = false;

        public Hero(Vector2 position, SpriteFont font = null) : base(LayerManager.Instance.EntityLayer, null, position, null, true, font)
        {

            DEBUG_SHOW_PIVOT = true;

            SetupAnimations();

            SetupController();

            foreach (Direction direction in new List<Direction>() { Direction.CENTER, Direction.UP, Direction.DOWN, Direction.LEFT, Direction.RIGHT })
            {
                CollisionCheckDirections.Add(direction);
            }

        }

        private void SetupAnimations()
        {
            Animations = new AnimationStateMachine();
            //Animations.Offset = new Vector2(3, -20);
            Animations.Offset = new Vector2(0, -27);

            CollisionOffsetRight = 0.5f;
            CollisionOffsetLeft = 0f;
            CollisionOffsetBottom = 0.4f;
            CollisionOffsetTop = 0.5f;

            Texture2D spriteSheet = SpriteUtil.LoadTexture("Green_Greens_Forest_Pixel_Art_Platformer_Pack/Character-Animations/Main-Character/Sprite-Sheets/main-character@hurt-sheet");
            SpriteSheetAnimation hurtRight = new SpriteSheetAnimation(this, spriteSheet, 1, 8, 8, 64, 64, 24);
            hurtRight.Looping = false;
            Animations.RegisterAnimation("HurtRight", hurtRight, () => false);

            SpriteSheetAnimation hurtLeft = new SpriteSheetAnimation(this, spriteSheet, 1, 8, 8, 64, 64, 24, SpriteEffects.FlipHorizontally);
            hurtLeft.Looping = false;
            Animations.RegisterAnimation("HurtLeft", hurtLeft, () => false);

            spriteSheet = SpriteUtil.LoadTexture("Green_Greens_Forest_Pixel_Art_Platformer_Pack/Character-Animations/Main-Character/Sprite-Sheets/main-character@idle-sheet");
            SpriteSheetAnimation idleRight = new SpriteSheetAnimation(this, spriteSheet, 3, 10, 24, 64, 64, 24);
            Func<bool> isIdleRight = () => CurrentFaceDirection == Direction.RIGHT;
            Animations.RegisterAnimation("IdleRight", idleRight, isIdleRight);

            SpriteSheetAnimation idleLeft = new SpriteSheetAnimation(this, spriteSheet, 3, 10, 24, 64, 64, 24, SpriteEffects.FlipHorizontally);
            Func<bool> isIdleLeft = () => CurrentFaceDirection == Direction.LEFT;
            Animations.RegisterAnimation("IdleLeft", idleLeft, isIdleLeft);

            spriteSheet = SpriteUtil.LoadTexture("Green_Greens_Forest_Pixel_Art_Platformer_Pack/Character-Animations/Main-Character/Sprite-Sheets/main-character@run-sheet");
            SpriteSheetAnimation runningRight = new SpriteSheetAnimation(this, spriteSheet, 1, 10, 10, 64, 64, 24);
            Func<bool> isRunningRight = () => Velocity.X > 0.5f && !CollisionChecker.HasBlockingColliderAt(GridUtil.GetRightGrid(GridCoordinates)) && (!onMovingPlatform || onMovingPlatform && UserInput.IsKeyPressed(Keys.Right));
            Animations.RegisterAnimation("RunningRight", runningRight, isRunningRight, 1);

            SpriteSheetAnimation runningLeft = new SpriteSheetAnimation(this, spriteSheet, 1, 10, 10, 64, 64, 24, SpriteEffects.FlipHorizontally);
            Func<bool> isRunningLeft = () => Velocity.X < -0.5f && !CollisionChecker.HasBlockingColliderAt(GridUtil.GetLeftGrid(GridCoordinates)) && (!onMovingPlatform || onMovingPlatform && UserInput.IsKeyPressed(Keys.Left));
            Animations.RegisterAnimation("RunningLeft", runningLeft, isRunningLeft, 1);

            SpriteSheetAnimation walingLeft = new SpriteSheetAnimation(this, spriteSheet, 1, 10, 10, 64, 64, 12, SpriteEffects.FlipHorizontally);
            Func<bool> isWalkingLeft = () => Velocity.X > -0.5f && Velocity.X < -0.1 && !CollisionChecker.HasBlockingColliderAt(GridUtil.GetLeftGrid(GridCoordinates)) && (!onMovingPlatform || onMovingPlatform && UserInput.IsKeyPressed(Keys.Left));
            Animations.RegisterAnimation("WalkingLeft", walingLeft, isWalkingLeft, 1);

            SpriteSheetAnimation walingRight = new SpriteSheetAnimation(this, spriteSheet, 1, 10, 10, 64, 64, 12);
            Func<bool> isWalkingRight = () => Velocity.X > 0.1 && Velocity.X < 0.5f && !CollisionChecker.HasBlockingColliderAt(GridUtil.GetLeftGrid(GridCoordinates)) && (!onMovingPlatform || onMovingPlatform && UserInput.IsKeyPressed(Keys.Right));
            Animations.RegisterAnimation("WalkingRight", walingRight, isWalkingRight, 1);

            Animations.AddFrameTransition("RunningRight", "WalkingRight");
            Animations.AddFrameTransition("RunningLeft", "WalkingLeft");

            spriteSheet = SpriteUtil.LoadTexture("Green_Greens_Forest_Pixel_Art_Platformer_Pack/Character-Animations/Main-Character/Sprite-Sheets/main-character@jump-sheet");
            SpriteSheetAnimation jumpRight = new SpriteSheetAnimation(this, spriteSheet, 2, 10, 11, 64, 64, 24);
            jumpRight.Looping = false;
            Func<bool> isJumpingRight = () => FallStartedAt > 0f && CurrentFaceDirection == Direction.RIGHT;
            Animations.RegisterAnimation("JumpingRight", jumpRight, isJumpingRight, 2);

            SpriteSheetAnimation jumpLeft = new SpriteSheetAnimation(this, spriteSheet, 2, 10, 11, 64, 64, 24, SpriteEffects.FlipHorizontally);
            jumpLeft.Looping = false;
            Func<bool> isJumpingLeft = () => FallStartedAt > 0f && CurrentFaceDirection == Direction.LEFT;
            Animations.RegisterAnimation("JumpingLeft", jumpLeft, isJumpingLeft, 2);

            Animations.AddFrameTransition("JumpingRight", "JumpingLeft");

            spriteSheet = SpriteUtil.LoadTexture("Green_Greens_Forest_Pixel_Art_Platformer_Pack/Character-Animations/Main-Character/Sprite-Sheets/main-character@wall-slide-sheet");
            SpriteSheetAnimation wallSlideRight = new SpriteSheetAnimation(this, spriteSheet, 1, 6, 6, 64, 64, 12, SpriteEffects.FlipHorizontally);
            Func<bool> isSlidingRight = () => JumpModifier != Vector2.Zero && CurrentFaceDirection == Direction.RIGHT;
            Animations.RegisterAnimation("WallSlideRight", wallSlideRight, isSlidingRight, 6);

            SpriteSheetAnimation wallSlideLeft = new SpriteSheetAnimation(this, spriteSheet, 1, 6, 6, 64, 64, 12);
            Func<bool> isSlidingLeft = () => JumpModifier != Vector2.Zero && CurrentFaceDirection == Direction.LEFT;
            Animations.RegisterAnimation("WallSlideLeft", wallSlideLeft, isSlidingLeft, 6);

            spriteSheet = SpriteUtil.LoadTexture("Green_Greens_Forest_Pixel_Art_Platformer_Pack/Character-Animations/Main-Character/Sprite-Sheets/main-character@double-jump-sheet");
            SpriteSheetAnimation doubleJumpRight = new SpriteSheetAnimation(this, spriteSheet, 3, 10, 16, 64, 64, 12);
            doubleJumpRight.StartFrame = 12;
            doubleJumpRight.EndFrame = 16;
            Func<bool> isDoubleJumpingRight = () => doubleJumping && CurrentFaceDirection == Direction.RIGHT;
            Animations.RegisterAnimation("DoubleJumpingRight", doubleJumpRight, isDoubleJumpingRight, 3);

            SpriteSheetAnimation doubleJumpLeft = new SpriteSheetAnimation(this, spriteSheet, 3, 10, 16, 64, 64, 12, SpriteEffects.FlipHorizontally);
            doubleJumpLeft.StartFrame = 12;
            doubleJumpLeft.EndFrame = 16;
            Func<bool> isDoubleJumpingLeft = () => doubleJumping && CurrentFaceDirection == Direction.LEFT;
            Animations.RegisterAnimation("DoubleJumpingLeft", doubleJumpLeft, isDoubleJumpingLeft, 3);

            Animations.AddFrameTransition("DoubleJumpingRight", "DoubleJumpingLeft");

            spriteSheet = SpriteUtil.LoadTexture("Green_Greens_Forest_Pixel_Art_Platformer_Pack/Character-Animations/Main-Character/Sprite-Sheets/main-character@climb-sheet");
            SpriteSheetAnimation climb = new SpriteSheetAnimation(this, spriteSheet, 2, 10, 12, 64, 64, 60);
            Func<bool> isClimbing = () => !HasGravity;
            Func<bool> isHangingOnLadder = () => (Math.Abs(Velocity.X) <= 0.1f && Math.Abs(Velocity.Y) <= 0.1f);
            climb.AnimationPauseCondition = isHangingOnLadder;
            Animations.RegisterAnimation("ClimbingLadder", climb, isClimbing, 6);

            SpriteSheetAnimation slowClimb = new SpriteSheetAnimation(this, spriteSheet, 2, 10, 12, 64, 64, 30);
            Func<bool> isSlowClimbing = () => !HasGravity && ((Math.Abs(Velocity.X) > 0.1 && Math.Abs(Velocity.X) < 0.5) || (Math.Abs(Velocity.Y) > 0.1 && Math.Abs(Velocity.Y) < 0.5));
            //Func<bool> isHangingOnLadder = () => (Math.Abs(Velocity.X) <= 0.1f && Math.Abs(Velocity.Y) <= 0.1f);
            climb.AnimationPauseCondition = isHangingOnLadder;
            Animations.RegisterAnimation("SlowClimbingLadder", slowClimb, isSlowClimbing, 7);

            spriteSheet = SpriteUtil.LoadTexture("Green_Greens_Forest_Pixel_Art_Platformer_Pack/Character-Animations/Main-Character/Sprite-Sheets/main-character@jump-sheet");
            SpriteSheetAnimation fallingRight = new SpriteSheetAnimation(this, spriteSheet, 2, 10, 13, 64, 64, 24);
            fallingRight.StartFrame = 9;
            fallingRight.EndFrame = 11;
            Func<bool> isFallingRight = () => HasGravity && Velocity.Y > 0.1 && CurrentFaceDirection == Direction.RIGHT;
            Animations.RegisterAnimation("FallingRight", fallingRight, isFallingRight, 5);

            SpriteSheetAnimation fallingLeft = new SpriteSheetAnimation(this, spriteSheet, 2, 10, 13, 64, 64, 24, SpriteEffects.FlipHorizontally);
            fallingLeft.StartFrame = 9;
            fallingLeft.EndFrame = 11;
            Func<bool> isFallingLeft = () => HasGravity && Velocity.Y > 0.1 && CurrentFaceDirection == Direction.LEFT;
            Animations.RegisterAnimation("FallingLeft", fallingLeft, isFallingLeft, 5);

            Animations.AddFrameTransition("FallingRight", "FallingLeft");

            //SetSprite(spriteSheet);
            /*SetSprite(SpriteUtil.CreateRectangle(Config.GRID, Color.Red));
            DrawOffset = new Vector2(15, 15);*/
        }

        private void SetupController()
        {
            UserInput = new UserInputController();

            UserInput.RegisterControllerState(Keys.R, (Vector2 thumbStickPosition) => {
                ResetPosition(new Vector2(200, 200));
            }, true);

            UserInput.RegisterControllerState(Keys.Right, Buttons.LeftThumbstickRight,(Vector2 thumbStickPosition) => {
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
                //CurrentFaceDirection = Direction.RIGHT;
            });

            UserInput.RegisterControllerState(Keys.Left, Buttons.LeftThumbstickLeft, (Vector2 thumbStickPosition) => {
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
                //CurrentFaceDirection = Direction.LEFT;
            });

            UserInput.RegisterControllerState(Keys.Up, Buttons.A, (Vector2 thumbStickPosition) => {
                if (!HasGravity || (!canJump && !canDoubleJump))
                {
                    return;
                }
                if (canJump)
                {
                    canDoubleJump = true;
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

                Velocity.Y -= Config.JUMP_FORCE + JumpModifier.Y;
                Velocity.X += JumpModifier.X;
                if (JumpModifier.X < 0)
                {
                    CurrentFaceDirection = Direction.LEFT;
                } else if (JumpModifier.X > 0)
                {
                    CurrentFaceDirection = Direction.RIGHT;
                }
                JumpModifier = Vector2.Zero;
                FallStartedAt = (float)GameTime.TotalGameTime.TotalSeconds;
            }, true);

            UserInput.RegisterControllerState(Keys.Down, Buttons.LeftThumbstickDown, (Vector2 thumbStickPosition) => {
                if (HasGravity)
                {
                    if (CollisionChecker.HasObjectAtWithTag(GridUtil.GetBelowGrid(GridCoordinates), "Platform") && CollisionChecker.GetColliderAt(GridUtil.GetBelowGrid(GridCoordinates)).BlocksMovement) {
                        CollisionChecker.GetColliderAt(GridUtil.GetBelowGrid(GridCoordinates)).BlocksMovement = false;
                    }
                }
                //CurrentFaceDirection = GridDirection.DOWN;
            }, true);

            UserInput.RegisterControllerState(Keys.Down, Buttons.LeftThumbstickDown, (Vector2 thumbStickPosition) => {
                if (HasGravity)
                {
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

            UserInput.RegisterControllerState(Keys.Up, Buttons.LeftThumbstickUp, (Vector2 thumbStickPosition) => {
                if (HasGravity && (GetSamePositionCollider() == null || !GetSamePositionCollider().HasTag("Ladder")))
                {
                    return;
                }

                if (HasGravity)
                {
                    Velocity.Y -= Config.JUMP_FORCE;
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

            UserInput.RegisterMouseActions(() => { Config.ZOOM += 0.5f; /*Globals.Camera.Recenter(); */ }, () => { Config.ZOOM -= 0.5f; /*Globals.Camera.Recenter(); */});
        }

        public override void Update(GameTime gameTime)
        {
            if (FallStartedAt > 0)
            {
                lastJump += gameTime.ElapsedGameTime.TotalSeconds;
            } else
            {
                doubleJumping = false;
            }
            base.Update(gameTime);
        }

        protected override void OnCollisionStart(Entity otherCollider, Direction direction)
        {
            if (otherCollider is Coin)
            {
                otherCollider.Destroy();
            } else if (otherCollider.HasTag("MovingPlatform"))
            {
                onMovingPlatform = true;
            }
            else if (otherCollider is Spring && direction == Direction.CENTER)
            {
                ((Spring)otherCollider).PlayBounceAnimation();
                /*if (Velocity.Y > 0)
                {
                    Velocity.Y = 0;
                    Velocity.Y -= ((Spring)otherCollider).Power + Velocity.Y;
                } else
                {
                    Velocity.Y = 0;
                    Velocity.Y -= ((Spring)otherCollider).Power;
                }*/

                Velocity.Y = 0;
                Velocity.Y -= ((Spring)otherCollider).Power;
            }
            else if(otherCollider is Carrot)
            {
                if (Timer.IsCounting("Damage"))
                {
                    return;
                }

                Timer.StartTimer("Damage", (float)TimeSpan.FromSeconds(1).TotalMilliseconds);

                if (CurrentFaceDirection == Direction.LEFT)
                {
                    Animations.PlayAnimation("HurtLeft");
                } else if (CurrentFaceDirection == Direction.RIGHT)
                {
                    Animations.PlayAnimation("HurtRight");
                }
                if (direction == Direction.LEFT)
                {
                    Velocity += new Vector2(2, -2);
                } else if (direction == Direction.RIGHT)
                {
                    Velocity += new Vector2(-2, -2);
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

        protected override void OnCollision(Entity otherCollider, Direction direction)
        {
            if (otherCollider.HasTag("MovingPlatform"))
            {
                Velocity.X += otherCollider.Velocity.X * elapsedTime;
            }
        }

        protected override void OnCollisionEnd(Entity otherCollider, Direction direction)
        {
            if (otherCollider.HasTag("Platform") && !otherCollider.BlocksMovement)
            {
                otherCollider.BlocksMovement = true;
            }
            else if (otherCollider.HasTag("MovingPlatform"))
            {
                onMovingPlatform = false;
            }
            else if (otherCollider is Spring && direction == Direction.CENTER)
            {
                FallStartedAt = 0;
            }
        }
    }
}
