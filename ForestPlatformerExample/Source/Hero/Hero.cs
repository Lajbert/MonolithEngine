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
            Texture2D spiteSheet = SpriteUtil.LoadTexture("Green_Greens_Forest_Pixel_Art_Platformer_Pack/Character-Animations/Main-Character/Sprite-Sheets/main-character@idle-sheet");
            SpriteSheetAnimation idleRight = new SpriteSheetAnimation(this, spiteSheet, 3, 10, 24, 64, 64, 24);
            Animations.Offset = new Vector2(0, -20);
            Func<bool> isIdleRight = () => CurrentFaceDirection == GameEngine2D.Engine.Source.Entities.Direction.RIGHT;
            Animations.RegisterAnimation("IdleRight", idleRight, isIdleRight);

            SpriteSheetAnimation idleLeft = new SpriteSheetAnimation(this, spiteSheet, 3, 10, 24, 64, 64, 24, SpriteEffects.FlipHorizontally);
            Animations.Offset = new Vector2(0, -20);
            Func<bool> isIdleLeft = () => CurrentFaceDirection == GameEngine2D.Engine.Source.Entities.Direction.LEFT;
            Animations.RegisterAnimation("IdleLeft", idleLeft, isIdleLeft);

            spiteSheet = SpriteUtil.LoadTexture("Green_Greens_Forest_Pixel_Art_Platformer_Pack/Character-Animations/Main-Character/Sprite-Sheets/main-character@run-sheet");
            SpriteSheetAnimation runningRight = new SpriteSheetAnimation(this, spiteSheet, 1, 10, 10, 64, 64, 24);
            Animations.Offset = new Vector2(0, -20);
            Func<bool> isRunningRight = () => Direction.X > 0.5f && !CollisionChecker.HasColliderAt(GridUtil.GetRightGrid(GridCoordinates));
            Animations.RegisterAnimation("RunningRight", runningRight, isRunningRight, 1);

            SpriteSheetAnimation runningLeft = new SpriteSheetAnimation(this, spiteSheet, 1, 10, 10, 64, 64, 24, SpriteEffects.FlipHorizontally);
            Animations.Offset = new Vector2(0, -20);
            Func<bool> isRunningLeft = () => Direction.X < -0.5f && !CollisionChecker.HasColliderAt(GridUtil.GetLeftGrid(GridCoordinates));
            Animations.RegisterAnimation("RunningLeft", runningLeft, isRunningLeft, 1);

            spiteSheet = SpriteUtil.LoadTexture("Green_Greens_Forest_Pixel_Art_Platformer_Pack/Character-Animations/Main-Character/Sprite-Sheets/main-character@jump-sheet");
            SpriteSheetAnimation jumpRight = new SpriteSheetAnimation(this, spiteSheet, 2, 10, 13, 64, 64, 24);
            Animations.Offset = new Vector2(0, -20);
            Func<bool> isJumpingRight = () => JumpStartedAt > 0f && CurrentFaceDirection == GameEngine2D.Engine.Source.Entities.Direction.RIGHT;
            Animations.RegisterAnimation("JumpingRight", jumpRight, isJumpingRight, 2);

            SpriteSheetAnimation jumpLeft = new SpriteSheetAnimation(this, spiteSheet, 2, 10, 13, 64, 64, 24, SpriteEffects.FlipHorizontally);
            Animations.Offset = new Vector2(0, -20);
            Func<bool> isJumpingLeft = () => JumpStartedAt > 0f && CurrentFaceDirection == GameEngine2D.Engine.Source.Entities.Direction.LEFT;
            Animations.RegisterAnimation("JumpingLeft", jumpLeft, isJumpingLeft, 2);

            spiteSheet = SpriteUtil.LoadTexture("Green_Greens_Forest_Pixel_Art_Platformer_Pack/Character-Animations/Main-Character/Sprite-Sheets/main-character@double-jump-sheet");
            SpriteSheetAnimation doubleJumpRight = new SpriteSheetAnimation(this, spiteSheet, 3, 10, 21, 64, 64, 30);
            Animations.Offset = new Vector2(0, -20);
            Func<bool> isDoubleJumpingRight = () => doubleJumping && CurrentFaceDirection == GameEngine2D.Engine.Source.Entities.Direction.RIGHT;
            //Func<bool> isDoubleJumpingRight = () => JumpStartedAt > 0f;
            Animations.RegisterAnimation("DoubleJumpingRight", doubleJumpRight, isDoubleJumpingRight, 3);

            SpriteSheetAnimation doubleJumpLeft = new SpriteSheetAnimation(this, spiteSheet, 3, 10, 21, 64, 64, 30, SpriteEffects.FlipHorizontally);
            Animations.Offset = new Vector2(0, -20);
            Func<bool> isDoubleJumpingLeft = () => doubleJumping && CurrentFaceDirection == GameEngine2D.Engine.Source.Entities.Direction.LEFT;
            //Func<bool> isDoubleJumpingLeft = () => JumpStartedAt > 0f;
            Animations.RegisterAnimation("DoubleJumpingLeft", doubleJumpLeft, isDoubleJumpingLeft, 3);




            Func<bool> isFallingRight = () => Direction.Y > 0f && CurrentFaceDirection == GameEngine2D.Engine.Source.Entities.Direction.RIGHT;

            Func<bool> isFallingLeftt = () => Direction.Y > 0f && CurrentFaceDirection == GameEngine2D.Engine.Source.Entities.Direction.LEFT;

            CollisionOffsetRight = 0f;
            CollisionOffsetLeft = 0f;
            CollisionOffsetBottom = 0f;
            CollisionOffsetTop = 0f;

            //SetSprite(spiteSheet);
        }

        private void SetupController()
        {
            UserInput = new UserInputController();

            UserInput.RegisterControllerState(Keys.Right, () => {
                Direction.X += Config.CHARACTER_SPEED * elapsedTime;
                CurrentFaceDirection = GameEngine2D.Engine.Source.Entities.Direction.RIGHT;
            });

            UserInput.RegisterControllerState(Keys.Left, () => {
                Direction.X -= Config.CHARACTER_SPEED * elapsedTime;
                CurrentFaceDirection = GameEngine2D.Engine.Source.Entities.Direction.LEFT;
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
                Direction.Y -= Config.JUMP_FORCE;
                JumpStartedAt = (float)gameTime.TotalGameTime.TotalSeconds;
            }, true);

            UserInput.RegisterControllerState(Keys.Down, () => {
                if (HasGravity)
                {
                    return;
                }
                Direction.Y += Config.CHARACTER_SPEED * elapsedTime;
                CurrentFaceDirection = GameEngine2D.Engine.Source.Entities.Direction.DOWN;
            });

            UserInput.RegisterControllerState(Keys.Up, () => {
                if (HasGravity)
                {
                    return;
                }
                Direction.Y -= Config.CHARACTER_SPEED * elapsedTime;
                CurrentFaceDirection = GameEngine2D.Engine.Source.Entities.Direction.UP;
            });

            UserInput.RegisterMouseActions(() => { Config.ZOOM += 0.1f; /*Globals.Camera.Recenter(); */ }, () => { Config.ZOOM -= 0.1f; /*Globals.Camera.Recenter(); */});
        }

        public override void Update(GameTime gameTime)
        {
            if (JumpStartedAt > 0)
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
