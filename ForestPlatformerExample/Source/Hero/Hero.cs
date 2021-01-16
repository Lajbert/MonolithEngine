using GameEngine2D;
using GameEngine2D.Engine.Source.Entities;
using GameEngine2D.Engine.Source.Entities.Animations;
using GameEngine2D.Engine.Source.Entities.Controller;
using GameEngine2D.Engine.Source.Util;
using GameEngine2D.Entities;
using GameEngine2D.Global;
using GameEngine2D.Source.Entities;
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


        private readonly float JUMP_RATE = 0.5f;
        private static double lastJump = 0f;
        private bool doubleJumping = false;

        public Hero(Vector2 position, SpriteFont font = null) : base(LayerManager.Instance.EntityLayer, null, position, null, true, font)
        {

            SetupAnimations();

            SetupController();

        }

        private void SetupAnimations()
        {
            Animations = new AnimationStateMachine();
            Animations.Offset = new Vector2(0, -20);

            Texture2D spiteSheet = SpriteUtil.LoadTexture("Green_Greens_Forest_Pixel_Art_Platformer_Pack/Character-Animations/Main-Character/Sprite-Sheets/main-character@idle-sheet");
            SpriteSheetAnimation idleRight = new SpriteSheetAnimation(this, spiteSheet, 3, 10, 24, 64, 64, 24);
            
            Func<bool> isIdleRight = () => CurrentFaceDirection == GridDirection.RIGHT;
            Animations.RegisterAnimation("IdleRight", idleRight, isIdleRight);

            SpriteSheetAnimation idleLeft = new SpriteSheetAnimation(this, spiteSheet, 3, 10, 24, 64, 64, 24, SpriteEffects.FlipHorizontally);
            Func<bool> isIdleLeft = () => CurrentFaceDirection == GridDirection.LEFT;
            Animations.RegisterAnimation("IdleLeft", idleLeft, isIdleLeft);

            spiteSheet = SpriteUtil.LoadTexture("Green_Greens_Forest_Pixel_Art_Platformer_Pack/Character-Animations/Main-Character/Sprite-Sheets/main-character@run-sheet");
            SpriteSheetAnimation runningRight = new SpriteSheetAnimation(this, spiteSheet, 1, 10, 10, 64, 64, 24);
            Func<bool> isRunningRight = () => Direction.X > 0.5f && !CollisionChecker.HasBlockingColliderAt(GridUtil.GetRightGrid(GridCoordinates));
            Animations.RegisterAnimation("RunningRight", runningRight, isRunningRight, 1);

            SpriteSheetAnimation runningLeft = new SpriteSheetAnimation(this, spiteSheet, 1, 10, 10, 64, 64, 24, SpriteEffects.FlipHorizontally);
            Func<bool> isRunningLeft = () => Direction.X < -0.5f && !CollisionChecker.HasBlockingColliderAt(GridUtil.GetLeftGrid(GridCoordinates));
            Animations.RegisterAnimation("RunningLeft", runningLeft, isRunningLeft, 1);

            spiteSheet = SpriteUtil.LoadTexture("Green_Greens_Forest_Pixel_Art_Platformer_Pack/Character-Animations/Main-Character/Sprite-Sheets/main-character@jump-sheet");
            SpriteSheetAnimation jumpRight = new SpriteSheetAnimation(this, spiteSheet, 2, 10, 11, 64, 64, 24);
            jumpRight.Looping = false;
            Func<bool> isJumpingRight = () => FallStartedAt > 0f && CurrentFaceDirection == GridDirection.RIGHT;
            Animations.RegisterAnimation("JumpingRight", jumpRight, isJumpingRight, 2);

            SpriteSheetAnimation jumpLeft = new SpriteSheetAnimation(this, spiteSheet, 2, 10, 11, 64, 64, 24, SpriteEffects.FlipHorizontally);
            jumpLeft.Looping = false;
            Func<bool> isJumpingLeft = () => FallStartedAt > 0f && CurrentFaceDirection == GridDirection.LEFT;
            Animations.RegisterAnimation("JumpingLeft", jumpLeft, isJumpingLeft, 2);

            Animations.AddFrameTransition("JumpingRight", "JumpingLeft");

            spiteSheet = SpriteUtil.LoadTexture("Green_Greens_Forest_Pixel_Art_Platformer_Pack/Character-Animations/Main-Character/Sprite-Sheets/main-character@wall-slide-sheet");
            SpriteSheetAnimation wallSlideRight = new SpriteSheetAnimation(this, spiteSheet, 1, 6, 6, 64, 64, 12, SpriteEffects.FlipHorizontally);
            Func<bool> isSlidingRight = () => JumpModifier != Vector2.Zero && CurrentFaceDirection == GridDirection.RIGHT;
            Animations.RegisterAnimation("WallSlideRight", wallSlideRight, isSlidingRight, 4);

            SpriteSheetAnimation wallSlideLeft = new SpriteSheetAnimation(this, spiteSheet, 1, 6, 6, 64, 64, 12);
            Func<bool> isSlidingLeft = () => JumpModifier != Vector2.Zero && CurrentFaceDirection == GridDirection.LEFT;
            Animations.RegisterAnimation("WallSlideLeft", wallSlideLeft, isSlidingLeft, 4);

            spiteSheet = SpriteUtil.LoadTexture("Green_Greens_Forest_Pixel_Art_Platformer_Pack/Character-Animations/Main-Character/Sprite-Sheets/main-character@double-jump-sheet");
            SpriteSheetAnimation doubleJumpRight = new SpriteSheetAnimation(this, spiteSheet, 3, 10, 16, 64, 64, 12);
            doubleJumpRight.Looping = false;
            doubleJumpRight.StartFrame = 12;
            doubleJumpRight.EndFrame = 16;
            Func<bool> isDoubleJumpingRight = () => doubleJumping && CurrentFaceDirection == GridDirection.RIGHT;
            Animations.RegisterAnimation("DoubleJumpingRight", doubleJumpRight, isDoubleJumpingRight, 3);

            SpriteSheetAnimation doubleJumpLeft = new SpriteSheetAnimation(this, spiteSheet, 3, 10, 16, 64, 64, 12, SpriteEffects.FlipHorizontally);
            doubleJumpLeft.Looping = false;
            doubleJumpLeft.StartFrame = 12;
            doubleJumpLeft.EndFrame = 16;
            Func<bool> isDoubleJumpingLeft = () => doubleJumping && CurrentFaceDirection == GridDirection.LEFT;
            Animations.RegisterAnimation("DoubleJumpingLeft", doubleJumpLeft, isDoubleJumpingLeft, 3);

            Animations.AddFrameTransition("DoubleJumpingRight", "DoubleJumpingLeft");

            spiteSheet = SpriteUtil.LoadTexture("Green_Greens_Forest_Pixel_Art_Platformer_Pack/Character-Animations/Main-Character/Sprite-Sheets/main-character@climb-sheet");
            SpriteSheetAnimation climb = new SpriteSheetAnimation(this, spiteSheet, 2, 10, 12, 64, 64, 30);
            Func<bool> isClimbing = () => !HasGravity;
            Func<bool> isHangingOnLadder = () => (Math.Abs(Direction.X) < 0.5f && Math.Abs(Direction.Y) < 0.5f);
            climb.AnimationPauseCondition = isHangingOnLadder;
            Animations.RegisterAnimation("WallSlideRight", climb, isClimbing, 5);

            CollisionOffsetRight = 0.5f;
            CollisionOffsetLeft = 0.2f;
            CollisionOffsetBottom = 0f;
            CollisionOffsetTop = 0f;

            //SetSprite(spiteSheet);
        }

        private void SetupController()
        {
            UserInput = new UserInputController();

            UserInput.RegisterControllerState(Keys.Right, () => {
                Direction.X += MovementSpeed * elapsedTime;
                CurrentFaceDirection = GridDirection.RIGHT;
            });

            UserInput.RegisterControllerState(Keys.Left, () => {
                Direction.X -= MovementSpeed * elapsedTime;
                CurrentFaceDirection = GridDirection.LEFT;
            });

            UserInput.RegisterControllerState(Keys.Space, () => {
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
                Direction.Y -= Config.JUMP_FORCE + JumpModifier.Y;
                Direction.X += JumpModifier.X;
                if (JumpModifier.X < 0)
                {
                    CurrentFaceDirection = GridDirection.LEFT;
                } else if (JumpModifier.X > 0)
                {
                    CurrentFaceDirection = GridDirection.RIGHT;
                }
                JumpModifier = Vector2.Zero;
                FallStartedAt = (float)gameTime.TotalGameTime.TotalSeconds;
            }, true);

            UserInput.RegisterControllerState(Keys.Down, () => {
                if (HasGravity)
                {
                    return;
                }
                Direction.Y += MovementSpeed * elapsedTime;
                //CurrentFaceDirection = GridDirection.DOWN;
            });

            UserInput.RegisterControllerState(Keys.Up, () => {
                if (HasGravity)
                {
                    return;
                }
                Direction.Y -= MovementSpeed * elapsedTime;
                //CurrentFaceDirection = GridDirection.UP;
            });

            UserInput.RegisterMouseActions(() => { Config.ZOOM += 0.1f; /*Globals.Camera.Recenter(); */ }, () => { Config.ZOOM -= 0.1f; /*Globals.Camera.Recenter(); */});
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
    }
}
