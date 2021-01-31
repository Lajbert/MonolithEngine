using ForestPlatformerExample.Source.Enemies;
using ForestPlatformerExample.Source.Items;
using ForestPlatformerExample.Source.Weapons;
using GameEngine2D;
using GameEngine2D.Engine.Source.Entities;
using GameEngine2D.Engine.Source.Entities.Animations;
using GameEngine2D.Engine.Source.Entities.Controller;
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

namespace ForestPlatformerExample.Source.Hero
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

        private bool isAttacking = false;

        private float climbSpeed = Config.CHARACTER_SPEED / 2;

        private Fist fist;

        public Hero(Vector2 position, SpriteFont font = null) : base(LayerManager.Instance.EntityLayer, null, position, null, font)
        {

            //DEBUG_SHOW_PIVOT = true;
            //DEBUG_SHOW_CIRCLE_COLLIDER = true;

            CircleCollider = new CircleCollider(this, 10, new Vector2(0, -10));

            //ColliderOnGrid = true;

            //RayEmitter = new Ray2DEmitter(this);

            SetupAnimations();

            SetupController();

            CollisionChecker.RestrictDirectionsForTag("Ladder", new HashSet<Direction> { Direction.UP, Direction.CENTER });

            foreach (Direction direction in new List<Direction>() { Direction.CENTER, Direction.UP, Direction.DOWN, Direction.LEFT, Direction.RIGHT })
            {
                GridCollisionCheckDirections.Add(direction);
            }

            fist = new Fist(this, new Vector2(10, -3));

        }

        private Texture2D red = SpriteUtil.CreateRectangle(Config.GRID, Color.Red);
        private Texture2D blue = SpriteUtil.CreateRectangle(Config.GRID, Color.Blue);
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

            SpriteSheetAnimation hurtLeft = hurtRight.CopyFlipped();
            Animations.RegisterAnimation("HurtLeft", hurtLeft, () => false);



            spriteSheet = SpriteUtil.LoadTexture("Green_Greens_Forest_Pixel_Art_Platformer_Pack/Character-Animations/Main-Character/Sprite-Sheets/main-character@idle-sheet");
            SpriteSheetAnimation idleRight = new SpriteSheetAnimation(this, spriteSheet, 3, 10, 24, 64, 64, 24);
            Func<bool> isIdleRight = () => CurrentFaceDirection == Direction.RIGHT;
            Animations.RegisterAnimation("IdleRight", idleRight, isIdleRight);

            SpriteSheetAnimation idleLeft = idleRight.CopyFlipped();
            Func<bool> isIdleLeft = () => CurrentFaceDirection == Direction.LEFT;
            Animations.RegisterAnimation("IdleLeft", idleLeft, isIdleLeft);

            spriteSheet = SpriteUtil.LoadTexture("Green_Greens_Forest_Pixel_Art_Platformer_Pack/Character-Animations/Main-Character/Sprite-Sheets/main-character@run-sheet");
            SpriteSheetAnimation runningRight = new SpriteSheetAnimation(this, spriteSheet, 1, 10, 10, 64, 64, 24);
            Func<bool> isRunningRight = () => Velocity.X > 0.5f && !CollisionChecker.HasBlockingColliderAt(GridUtil.GetRightGrid(GridCoordinates)) && (!onMovingPlatform || onMovingPlatform && UserInput.IsKeyPressed(Keys.Right));
            Animations.RegisterAnimation("RunningRight", runningRight, isRunningRight, 1);

            SpriteSheetAnimation runningLeft = runningRight.CopyFlipped();
            Func<bool> isRunningLeft = () => Velocity.X < -0.5f && !CollisionChecker.HasBlockingColliderAt(GridUtil.GetLeftGrid(GridCoordinates)) && (!onMovingPlatform || onMovingPlatform && UserInput.IsKeyPressed(Keys.Left));
            Animations.RegisterAnimation("RunningLeft", runningLeft, isRunningLeft, 1);

            SpriteSheetAnimation walkingLeft = new SpriteSheetAnimation(this, spriteSheet, 1, 10, 10, 64, 64, 12, SpriteEffects.FlipHorizontally);
            Func<bool> isWalkingLeft = () => Velocity.X > -0.5f && Velocity.X < -0.1 && !CollisionChecker.HasBlockingColliderAt(GridUtil.GetLeftGrid(GridCoordinates)) && (!onMovingPlatform || onMovingPlatform && UserInput.IsKeyPressed(Keys.Left));
            Animations.RegisterAnimation("WalkingLeft", walkingLeft, isWalkingLeft, 1);

            SpriteSheetAnimation walkingRight = walkingLeft.CopyFlipped();
            Func<bool> isWalkingRight = () => Velocity.X > 0.1 && Velocity.X < 0.5f && !CollisionChecker.HasBlockingColliderAt(GridUtil.GetLeftGrid(GridCoordinates)) && (!onMovingPlatform || onMovingPlatform && UserInput.IsKeyPressed(Keys.Right));
            Animations.RegisterAnimation("WalkingRight", walkingRight, isWalkingRight, 1);

            Animations.AddFrameTransition("RunningRight", "WalkingRight");
            Animations.AddFrameTransition("RunningLeft", "WalkingLeft");

            spriteSheet = SpriteUtil.LoadTexture("Green_Greens_Forest_Pixel_Art_Platformer_Pack/Character-Animations/Main-Character/Sprite-Sheets/main-character@jump-sheet");
            SpriteSheetAnimation jumpRight = new SpriteSheetAnimation(this, spriteSheet, 2, 10, 11, 64, 64, 24);
            jumpRight.Looping = false;
            Func<bool> isJumpingRight = () => FallSpeed > 0f && CurrentFaceDirection == Direction.RIGHT;
            Animations.RegisterAnimation("JumpingRight", jumpRight, isJumpingRight, 2);

            SpriteSheetAnimation jumpLeft = jumpRight.CopyFlipped();
            Func<bool> isJumpingLeft = () => FallSpeed > 0f && CurrentFaceDirection == Direction.LEFT;
            Animations.RegisterAnimation("JumpingLeft", jumpLeft, isJumpingLeft, 2);

            Animations.AddFrameTransition("JumpingRight", "JumpingLeft");

            spriteSheet = SpriteUtil.LoadTexture("Green_Greens_Forest_Pixel_Art_Platformer_Pack/Character-Animations/Main-Character/Sprite-Sheets/main-character@wall-slide-sheet");
            SpriteSheetAnimation wallSlideRight = new SpriteSheetAnimation(this, spriteSheet, 1, 6, 6, 64, 64, 12, SpriteEffects.FlipHorizontally);
            Func<bool> isWallSlidingRight = () => jumpModifier != Vector2.Zero && CurrentFaceDirection == Direction.RIGHT;
            Animations.RegisterAnimation("WallSlideRight", wallSlideRight, isWallSlidingRight, 6);

            SpriteSheetAnimation wallSlideLeft = wallSlideRight.CopyFlipped();
            Func<bool> isWallSlidingLeft = () => jumpModifier != Vector2.Zero && CurrentFaceDirection == Direction.LEFT;
            Animations.RegisterAnimation("WallSlideLeft", wallSlideLeft, isWallSlidingLeft, 6);

            spriteSheet = SpriteUtil.LoadTexture("Green_Greens_Forest_Pixel_Art_Platformer_Pack/Character-Animations/Main-Character/Sprite-Sheets/main-character@double-jump-sheet");
            SpriteSheetAnimation doubleJumpRight = new SpriteSheetAnimation(this, spriteSheet, 3, 10, 16, 64, 64, 12);
            doubleJumpRight.StartFrame = 12;
            doubleJumpRight.EndFrame = 16;
            Func<bool> isDoubleJumpingRight = () => doubleJumping && CurrentFaceDirection == Direction.RIGHT;
            Animations.RegisterAnimation("DoubleJumpingRight", doubleJumpRight, isDoubleJumpingRight, 3);

            SpriteSheetAnimation doubleJumpLeft = doubleJumpRight.CopyFlipped();
            Func<bool> isDoubleJumpingLeft = () => doubleJumping && CurrentFaceDirection == Direction.LEFT;
            Animations.RegisterAnimation("DoubleJumpingLeft", doubleJumpLeft, isDoubleJumpingLeft, 3);

            Animations.AddFrameTransition("DoubleJumpingRight", "DoubleJumpingLeft");

            spriteSheet = SpriteUtil.LoadTexture("Green_Greens_Forest_Pixel_Art_Platformer_Pack/Character-Animations/Main-Character/Sprite-Sheets/main-character@climb-sheet");
            SpriteSheetAnimation climb = new SpriteSheetAnimation(this, spriteSheet, 2, 10, 12, 64, 64, 60);

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

            SpriteSheetAnimation slowClimb = new SpriteSheetAnimation(this, spriteSheet, 2, 10, 12, 64, 64, 15);
            Func<bool> isSlowClimbing = () => !HasGravity && ((Math.Abs(Velocity.X) > 0.01f && Math.Abs(Velocity.X) < 0.5) || (Math.Abs(Velocity.Y) > 0.01f && Math.Abs(Velocity.Y) < 0.5));
            slowClimb.EveryFrameAction = setSpeed;
            Animations.RegisterAnimation("SlowClimbingLadder", slowClimb, isSlowClimbing, 7);

            Animations.AddFrameTransition("ClimbingLadder", "SlowClimbingLadder");

            spriteSheet = SpriteUtil.LoadTexture("Green_Greens_Forest_Pixel_Art_Platformer_Pack/Character-Animations/Main-Character/Sprite-Sheets/main-character@jump-sheet");
            SpriteSheetAnimation fallingRight = new SpriteSheetAnimation(this, spriteSheet, 2, 10, 13, 64, 64, 24);
            fallingRight.StartFrame = 9;
            fallingRight.EndFrame = 11;
            Func<bool> isFallingRight = () => HasGravity && Velocity.Y > 0.1 && CurrentFaceDirection == Direction.RIGHT;
            Animations.RegisterAnimation("FallingRight", fallingRight, isFallingRight, 5);

            SpriteSheetAnimation fallingLeft = fallingRight.CopyFlipped();
            Func<bool> isFallingLeft = () => HasGravity && Velocity.Y > 0.1 && CurrentFaceDirection == Direction.LEFT;
            Animations.RegisterAnimation("FallingLeft", fallingLeft, isFallingLeft, 5);

            Animations.AddFrameTransition("FallingRight", "FallingLeft");

            spriteSheet = SpriteUtil.LoadTexture("Green_Greens_Forest_Pixel_Art_Platformer_Pack/Character-Animations/Main-Character/Sprite-Sheets/main-character@attack-sheet");
            SpriteSheetAnimation attackRight = new SpriteSheetAnimation(this, spriteSheet, 1, 8, 8, 64, 64, 48);
            //attackRight.StartedCallback = () => Velocity.X = 0f;
            attackRight.AddFrameAction(5, (frame) => canAttack = true);
            attackRight.AddFrameAction(5, (frame) => fist.IsAttacking = false);
            //attackRight.EveryFrameAction = (frame) => HitEnemy();
            //attackRight.StoppedCallback += () => isAttacking = false;
            attackRight.Looping = false;
            Animations.RegisterAnimation("AttackRight", attackRight, () => false, 8);

            SpriteSheetAnimation attackLeft = attackRight.CopyFlipped();
            Animations.RegisterAnimation("AttackLeft", attackLeft, () => false, 8);

            //SetSprite(spriteSheet);
            //SetSprite(blue);
            //DrawOffset = new Vector2(15, 15);
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
                    if (CollisionChecker.HasObjectAtWithTag(GridUtil.GetBelowGrid(GridCoordinates), "Platform") && CollisionChecker.GetColliderAt(GridUtil.GetBelowGrid(GridCoordinates)).BlocksMovement) {
                        CollisionChecker.GetColliderAt(GridUtil.GetBelowGrid(GridCoordinates)).BlocksMovement = false;
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

            UserInput.RegisterMouseActions(() => { Config.ZOOM += 0.5f; /*Globals.Camera.Recenter(); */ }, () => { Config.ZOOM -= 0.5f; /*Globals.Camera.Recenter(); */});
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

                /*Velocity.Y = 0;
                Friction = 0.5f;
                GravityValue = 0;
                Timer.TriggerAfter(200, SetSpringGravity, true);
                Velocity.Y -= ((Spring)otherCollider).Power;*/
                Bump(new Vector2(0, -15));
                canJump = false;
                canDoubleJump = true;
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
            else if (otherCollider.HasTag("Platform") && Velocity.Y >= 0) //|| direction != Direction.TOPLEFT || direction != Direction.TOPRIGHT))
            {
                otherCollider.BlocksMovement = true;
            } 
            else if (otherCollider.HasTag("SlideWall") && !OnGround())
            {
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

        private void SetSpringGravity()
        {
            GravityValue = Config.GRAVITY_FORCE;
            Friction = Config.FRICTION;
        }

        private float GetVelocity(float thumbStickPosition, float maxVelocity)
        {
            if (thumbStickPosition == 0)
            {
                return 0;
            }
            return thumbStickPosition / 1 * maxVelocity;
        }

        protected override void OnGridCollision(Entity otherCollider, Direction direction)
        {
            if (otherCollider.HasTag("MovingPlatform"))
            {
                Velocity.X += (otherCollider as PhysicalEntity).Velocity.X * elapsedTime;
            }
        }

        protected override void OnGridCollisionEnd(Entity otherCollider, Direction direction)
        {
            if (otherCollider.HasTag("MovingPlatform"))
            {
                onMovingPlatform = false;
            }
            else if (otherCollider is Spring && direction == Direction.CENTER)
            {
                FallSpeed = 0;
            }
            else if (otherCollider.HasTag("Ladder") && CollisionChecker.CollidesWithTag(GridCoordinates, "Ladder").Count == 0)
            {
                LeaveLadder();
            }
            else if (otherCollider.HasTag("Platform"))
            {
                otherCollider.BlocksMovement = false;
            } 
            else if (otherCollider.HasTag("SlideWall") && CollisionChecker.CollidesWithTag(GridCoordinates, "SlideWall").Count == 0)
            {
                GravityValue = Config.GRAVITY_FORCE;
                jumpModifier = Vector2.Zero;
                canAttack = true;
            }
        }

        protected override void OnCircleCollisionStart(Entity otherCollider, float intersection)
        {
            if (otherCollider is Carrot)
            {
                if (isAttacking)
                {
                    //(otherCollider as Carrot).Hit(CurrentFaceDirection);
                    return;
                }
                float angle = MathUtil.DegreeFromVectors(Position, otherCollider.Position);
                if (angle <= 135 && angle >= 45)
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
        }

        protected override void OnCircleCollisionEnd(Entity otherCollider)
        {
            //Logger.Log("HERO CIRCLE ENDED " + otherCollider);
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
