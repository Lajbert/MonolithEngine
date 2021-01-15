using GameEngine2D;
using GameEngine2D.Engine.Source.Entities.Animations;
using GameEngine2D.Engine.Source.Entities.Controller;
using GameEngine2D.Engine.Source.Util;
using GameEngine2D.Entities;
using GameEngine2D.Global;
using GameEngine2D.Source.Entities;
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
            //knightAnimationIdleRight.Scale = scale;
            //Func<bool> isIdleRight = () => CurrentFaceDirection == Engine.Source.Entities.Direction.RIGHT;
            Func<bool> isIdleRight = () => true;
            Animations.RegisterAnimation("IdleRight", idleRight, isIdleRight);

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
