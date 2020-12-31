using GameEngine2D.Engine.src.Entities.Animations;
using GameEngine2D.Engine.src.Util;
using GameEngine2D.Entities;
using GameEngine2D.GameExamples2D.SideScroller.src;
using GameEngine2D.Global;
using GameEngine2D.src;
using GameEngine2D.src.Entities.Animation;
using GameEngine2D.src.Layer;
using GameEngine2D.src.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.GameExamples.SideScroller.src.Hero
{
    class Knight : ControllableEntity
    {

        private float scale = 1.5f;
        private Rectangle sourceRectangle = new Rectangle(0, 0, 100, 55);
        private Vector2 spriteOffset = new Vector2(70f, 15f) * -1;
        private int fps = 30;

        private AnimationStateMachine animations;

        public Knight(Vector2 position, SpriteFont font = null) : base(Scene.Instance.GetEntityLayer(), null, position, font)
        {

            animations = new AnimationStateMachine();

            string folder = "SideScroller/KnightAssets/HeroKnight/";

            List<Texture2D> knightIdle = SpriteUtil.LoadTextures(folder + "Idle/HeroKnight_Idle_", 7, SideScrollerGame.contentManager);

            AnimatedSpriteGroup knightAnimationIdleRight = new AnimatedSpriteGroup(knightIdle, this, spriteBatch, sourceRectangle, fps);
            knightAnimationIdleRight.SetScale(scale);
            knightAnimationIdleRight.SetOffset(spriteOffset);
            Func<bool> isIdleRight = () => IsIdle() && faceDirection == FaceDirection.RIGHT;
            animations.RegisterStateAnimation("IdleRight", knightAnimationIdleRight, isIdleRight);

            AnimatedSpriteGroup knightAnimationIdleLeft = new AnimatedSpriteGroup(knightIdle, this, spriteBatch, sourceRectangle, fps, SpriteEffects.FlipHorizontally);
            knightAnimationIdleLeft.SetScale(scale);
            knightAnimationIdleLeft.SetOffset(spriteOffset);
            Func<bool> isIdleLeft = () => IsIdle() && faceDirection == FaceDirection.LEFT;
            animations.RegisterStateAnimation("idleLeft", knightAnimationIdleLeft, isIdleLeft);

            List<Texture2D> knightRun = SpriteUtil.LoadTextures(folder + "Run/HeroKnight_Run_", 7, SideScrollerGame.contentManager);
            AnimatedSpriteGroup knightRunRightAnimation = new AnimatedSpriteGroup(knightRun, this, spriteBatch, sourceRectangle, fps);
            knightRunRightAnimation.SetScale(scale);
            knightRunRightAnimation.SetOffset(spriteOffset);
            Func<bool> isRunningRight = () => direction.X > 0.5f;
            animations.RegisterStateAnimation("runRight", knightRunRightAnimation, isRunningRight);

            AnimatedSpriteGroup knightRunLeftAnimation = new AnimatedSpriteGroup(knightRun, this, spriteBatch, sourceRectangle, fps, SpriteEffects.FlipHorizontally);
            knightRunLeftAnimation.SetScale(scale);
            knightRunLeftAnimation.SetOffset(spriteOffset);
            Func<bool> isRunningleft = () => direction.X < 0.5f;
            animations.RegisterStateAnimation("runLeft", knightRunLeftAnimation, isRunningleft);
            
            List<Texture2D> knightJump = SpriteUtil.LoadTextures(folder + "Jump/HeroKnight_Jump_", 2, SideScrollerGame.contentManager);
            AnimatedSpriteGroup knightJumpRightAnimation = new AnimatedSpriteGroup(knightJump, this, spriteBatch, sourceRectangle, fps);
            knightJumpRightAnimation.SetScale(scale);
            knightJumpRightAnimation.SetOffset(spriteOffset);
            Func<bool> isJumpingRight = () => jumpStart > 0f && faceDirection == FaceDirection.RIGHT;
            animations.RegisterStateAnimation("jumpRight", knightJumpRightAnimation, isJumpingRight, 1);

            AnimatedSpriteGroup knightJumpLeftAnimation = new AnimatedSpriteGroup(knightJump, this, spriteBatch, sourceRectangle, fps, SpriteEffects.FlipHorizontally);
            knightJumpLeftAnimation.SetScale(scale);
            knightJumpLeftAnimation.SetOffset(spriteOffset);
            Func<bool> isJumpingLeft = () => jumpStart > 0f && faceDirection == FaceDirection.LEFT;
            animations.RegisterStateAnimation("jumpLeft", knightJumpLeftAnimation, isJumpingLeft, 1);

            List<Texture2D> knightFall = SpriteUtil.LoadTextures(folder + "Fall/HeroKnight_Fall_", 3, SideScrollerGame.contentManager);
            AnimatedSpriteGroup knightFallRightAnimation = new AnimatedSpriteGroup(knightFall, this, spriteBatch, sourceRectangle, fps);
            knightFallRightAnimation.SetScale(scale);
            knightFallRightAnimation.SetOffset(spriteOffset);
            Func<bool> isFallingRight = () => direction.Y > 0f && faceDirection == FaceDirection.RIGHT;
            animations.RegisterStateAnimation("fallRight", knightFallRightAnimation, isFallingRight, 2);

            AnimatedSpriteGroup knightFallLeftAnimation = new AnimatedSpriteGroup(knightFall, this, spriteBatch, sourceRectangle, fps, SpriteEffects.FlipHorizontally);
            knightFallLeftAnimation.SetScale(scale);
            knightFallLeftAnimation.SetOffset(spriteOffset);
            Func<bool> isFallingLeftt = () => direction.Y > 0f && faceDirection == FaceDirection.LEFT;
            animations.RegisterStateAnimation("fallLeft", knightFallLeftAnimation, isFallingLeftt, 2);

            //SetSprite(SpriteUtil.CreateRectangle(graphicsDevice, Constants.GRID, Color.Black));

            SetAnimationStateMachime(animations);
        }
    }
}
