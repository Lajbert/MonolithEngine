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



            Func<bool> isRunningleft = () => Direction.X < -0.5f && !CollisionChecker.HasColliderAt(GridUtil.GetLeftGrid(GridCoordinates));

            Func<bool> isJumpingRight = () => JumpStart > 0f && CurrentFaceDirection == GameEngine2D.Engine.Source.Entities.Direction.RIGHT;

            Func<bool> isJumpingLeft = () => JumpStart > 0f && CurrentFaceDirection == GameEngine2D.Engine.Source.Entities.Direction.LEFT;

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
                if (!HasGravity || (!canJump && !doubleJump))
                {
                    return;
                }

                if (canJump)
                {
                    doubleJump = true;
                    canJump = false;
                }
                else
                {
                    if (lastJump < JUMP_RATE)
                    {
                        return;
                    }
                    lastJump = 0f;
                    doubleJump = false;
                }
                Direction.Y -= Config.JUMP_FORCE;
                JumpStart = (float)gameTime.TotalGameTime.TotalSeconds;
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
            lastJump += gameTime.ElapsedGameTime.TotalSeconds;
            base.Update(gameTime);
        }
    }
}
