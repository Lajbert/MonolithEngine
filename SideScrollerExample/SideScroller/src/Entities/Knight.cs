using GameEngine2D.Engine.Source.Entities;
using GameEngine2D.Engine.Source.Entities.Animations;
using GameEngine2D.Engine.Source.Entities.Controller;
using GameEngine2D.Engine.Source.Util;
using GameEngine2D.Entities;
using GameEngine2D.Global;
using GameEngine2D.Source;
using GameEngine2D.Source.Entities.Animation;
using GameEngine2D.Source.Layer;
using GameEngine2D.Source.Util;
using GameEngine2D.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SideScrollerExample;
using SideScrollerExample.SideScroller.Source.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.GameExamples.SideScroller.Source.Hero
{
    class Knight : ControllableEntity
    {

        private readonly float SHOOT_RATE = 0.1f;

        //private float scale = 2.5f;
        private float scale = 1f;
        //private Rectangle sourceRectangle = new Rectangle(0, 0, 100, 55);
        private Vector2 spriteOffset = new Vector2(0, 0f);
        //private Vector2 spriteOffset = Vector2.Zero;
        private int animationFps = 30;

        private AnimationStateMachine animations;

        private static double lastBulletInSeconds = 0f;

        private SoundEffect shotEffect;

        public Knight(ContentManager contentManager, Vector2 position, SpriteFont font = null) : base(RootContainer.Instance.EntityLayer, null, position, null, true, font)
        {

            SetupAnimations(contentManager);
            CollisionOffsetRight = 0.5f;
            CollisionOffsetLeft = 0.5f;
            CollisionOffsetBottom = 0f;
            CollisionOffsetTop = 0.5f;
            SetupController();
            animations.Offset = new Vector2(0, -10f);
            shotEffect = contentManager.Load<SoundEffect>("Audio/Effects/GunShot");

        }

        private void SetupController()
        {
            UserInput = new UserInputController();

            UserInput.RegisterControllerState(Keys.Right, () => {
                Direction.X += Config.CHARACTER_SPEED * elapsedTime;
                CurrentFaceDirection = Engine.Source.Entities.Direction.RIGHT;
            });

            UserInput.RegisterControllerState(Keys.Left, () => {
                Direction.X -= Config.CHARACTER_SPEED * elapsedTime;
                CurrentFaceDirection = Engine.Source.Entities.Direction.LEFT;
            });

            UserInput.RegisterControllerState(Keys.Space, () => {
                if (!HasGravity || (!canJump && !doubleJump))
                {
                    return;
                }
                if (canJump)
                {
                    doubleJump = true;
                }
                else
                {
                    doubleJump = false;
                }
                canJump = false;
                Direction.Y -= Config.JUMP_FORCE;
                JumpStart = (float)gameTime.TotalGameTime.TotalSeconds;
            }, true);

            UserInput.RegisterControllerState(Keys.Down, () => {
                if (HasGravity)
                {
                    return;
                }
                Direction.Y += Config.CHARACTER_SPEED * elapsedTime;
                CurrentFaceDirection = Engine.Source.Entities.Direction.DOWN;
            });

            UserInput.RegisterControllerState(Keys.Up, () => {
                if (HasGravity)
                {
                    return;
                }
                Direction.Y -= Config.CHARACTER_SPEED * elapsedTime;
                CurrentFaceDirection = Engine.Source.Entities.Direction.UP;
            });

            Action shoot = () =>
            {
                if (lastBulletInSeconds >= SHOOT_RATE)
                {
                    lastBulletInSeconds = 0;
                    new Bullet(this, CurrentFaceDirection);
                    //shotEffect.Play();
                }
            };

            UserInput.RegisterControllerState(Keys.RightShift, shoot, true);
            UserInput.RegisterControllerState(Keys.LeftShift, shoot, true);

            UserInput.RegisterMouseActions(() => {Config.ZOOM += 0.1f; /*Globals.Camera.Recenter(); */ }, () => { Config.ZOOM -= 0.1f; /*Globals.Camera.Recenter(); */});
        }

        public override void Update(GameTime gameTime)
        {
            lastBulletInSeconds += gameTime.ElapsedGameTime.TotalSeconds;
            base.Update(gameTime);
        }

        private void SetupAnimations(ContentManager contentManager)
        {
            animations = new AnimationStateMachine();

            string folder = "SideScroller/KnightAssets/HeroKnight/";

            List<Texture2D> knightIdle = SpriteUtil.LoadTextures(folder + "Idle/HeroKnight_Idle_", 7);

            SpriteGroupAnimation knightAnimationIdleRight = new SpriteGroupAnimation(this, knightIdle, animationFps);
            knightAnimationIdleRight.Scale = scale;
            Func<bool> isIdleRight = () => CurrentFaceDirection == Engine.Source.Entities.Direction.RIGHT;
            animations.RegisterAnimation("IdleRight", knightAnimationIdleRight, isIdleRight);

            SpriteGroupAnimation knightAnimationIdleLeft = new SpriteGroupAnimation(this, knightIdle, animationFps, SpriteEffects.FlipHorizontally);
            knightAnimationIdleLeft.Scale = scale;
            Func<bool> isIdleLeft = () => CurrentFaceDirection == Engine.Source.Entities.Direction.LEFT;
            animations.RegisterAnimation("IdleLeft", knightAnimationIdleLeft, isIdleLeft);

            List<Texture2D> knightRun = SpriteUtil.LoadTextures(folder + "Run/HeroKnight_Run_", 9);
            SpriteGroupAnimation knightRunRightAnimation = new SpriteGroupAnimation(this, knightRun, animationFps);
            knightRunRightAnimation.Scale = scale;
            Func<bool> isRunningRight = () => Direction.X > 0.5f && !CollisionChecker.HasColliderAt(GridUtil.GetRightGrid(GridCoordinates));
            animations.RegisterAnimation("RunRight", knightRunRightAnimation, isRunningRight, 1);

            SpriteGroupAnimation knightRunLeftAnimation = new SpriteGroupAnimation(this, knightRun, animationFps, SpriteEffects.FlipHorizontally);
            knightRunLeftAnimation.Scale = scale;
            Func<bool> isRunningleft = () => Direction.X < -0.5f && !CollisionChecker.HasColliderAt(GridUtil.GetLeftGrid(GridCoordinates));
            animations.RegisterAnimation("RunLeft", knightRunLeftAnimation, isRunningleft, 1);

            List<Texture2D> knightJump = SpriteUtil.LoadTextures(folder + "Jump/HeroKnight_Jump_", 2);
            SpriteGroupAnimation knightJumpRightAnimation = new SpriteGroupAnimation(this, knightJump, animationFps);
            knightJumpRightAnimation.Scale = scale;
            Func<bool> isJumpingRight = () => JumpStart > 0f && CurrentFaceDirection == Engine.Source.Entities.Direction.RIGHT;
            animations.RegisterAnimation("JumpRight", knightJumpRightAnimation, isJumpingRight, 2);

            SpriteGroupAnimation knightJumpLeftAnimation = new SpriteGroupAnimation(this, knightJump, animationFps, SpriteEffects.FlipHorizontally);
            knightJumpLeftAnimation.Scale = scale;
            Func<bool> isJumpingLeft = () => JumpStart > 0f && CurrentFaceDirection == Engine.Source.Entities.Direction.LEFT;
            animations.RegisterAnimation("JumpLeft", knightJumpLeftAnimation, isJumpingLeft, 2);

            List<Texture2D> knightFall = SpriteUtil.LoadTextures(folder + "Fall/HeroKnight_Fall_", 3);
            SpriteGroupAnimation knightFallRightAnimation = new SpriteGroupAnimation(this, knightFall, animationFps);
            knightFallRightAnimation.Scale = scale;
            knightFallRightAnimation.Offset = spriteOffset;
            Func<bool> isFallingRight = () => Direction.Y > 0f && CurrentFaceDirection == Engine.Source.Entities.Direction.RIGHT;
            animations.RegisterAnimation("FallRight", knightFallRightAnimation, isFallingRight, 3);

            SpriteGroupAnimation knightFallLeftAnimation = new SpriteGroupAnimation(this, knightFall, animationFps, SpriteEffects.FlipHorizontally);
            knightFallLeftAnimation.Scale = scale;
            Func<bool> isFallingLeftt = () => Direction.Y > 0f && CurrentFaceDirection == Engine.Source.Entities.Direction.LEFT;
            animations.RegisterAnimation("FallLeft", knightFallLeftAnimation, isFallingLeftt, 3);

            Animations = animations;

            //SetSprite(SpriteUtil.LoadTexturesWithMerge(folder + "Run/HeroKnight_Run_", 9));

            /*SetSprite(SpriteUtil.CreateRectangle(Config.GRID, Color.Black));
            Pivot = new Vector2(Sprite.Width / 2, Sprite.Height / 2);
            CollisionOffsetRight = 0.5f;
            CollisionOffsetLeft = 0.5f;
            CollisionOffsetBottom = 0.5f;
            CollisionOffsetTop = 0.5f;*/
        }
    }
}
