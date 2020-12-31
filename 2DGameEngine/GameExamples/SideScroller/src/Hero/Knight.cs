using GameEngine2D.Engine.src.Entities.Animations;
using GameEngine2D.Engine.src.Util;
using GameEngine2D.Entities;
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

        public Knight(GraphicsDeviceManager graphicsDevice, ContentManager content, SpriteBatch spriteBatch, Vector2 position, SpriteFont font = null) : base(Scene.Instance.GetEntityLayer(), null, graphicsDevice.GraphicsDevice, position, font)
        {

            animations = new AnimationStateMachine();

            string folder = "SideScroller/KnightAssets/HeroKnight/Idle/";

            List<Texture2D> knightIdle = new List<Texture2D>();
            
            for (int i = 0; i <= 7; i++)
            {
                Texture2D t = content.Load<Texture2D>(folder + "HeroKnight_Idle_" + i);
                knightIdle.Add(t);
            }
            AnimatedSpriteGroup knightAnimationIdleRight = new AnimatedSpriteGroup(knightIdle, this, spriteBatch, sourceRectangle, fps);
            knightAnimationIdleRight.SetScale(scale);
            knightAnimationIdleRight.SetOffset(spriteOffset);
            Func<bool> isIdleRight = () => IsIdle() && faceDirection == FaceDirection.RIGHT;
            animations.RegisterStateAnimation("idleRight", knightAnimationIdleRight, isIdleRight);

            AnimatedSpriteGroup knightAnimationIdleLeft = new AnimatedSpriteGroup(knightIdle, this, spriteBatch, sourceRectangle, fps, SpriteEffects.FlipHorizontally);
            knightAnimationIdleLeft.SetScale(scale);
            knightAnimationIdleLeft.SetOffset(spriteOffset);
            Func<bool> isIdleLeft = () => IsIdle() && faceDirection == FaceDirection.LEFT;
            animations.RegisterStateAnimation("idleRight", knightAnimationIdleLeft, isIdleLeft);

            List<Texture2D> knightRun = new List<Texture2D>();
            folder = "SideScroller/KnightAssets/HeroKnight/Run/";
            for (int i = 0; i <= 7; i++)
            {
                Texture2D t = content.Load<Texture2D>(folder + "HeroKnight_Run_" + i);
                knightRun.Add(t);
            }
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
            
            List<Texture2D> knightJump = new List<Texture2D>();
            folder = "SideScroller/KnightAssets/HeroKnight/Jump/";
            for (int i = 0; i <= 2; i++)
            {
                Texture2D t = content.Load<Texture2D>(folder + "HeroKnight_Jump_" + i);
                knightJump.Add(t);
            }
            AnimatedSpriteGroup knightJumpRightAnimation = new AnimatedSpriteGroup(knightJump, this, spriteBatch, sourceRectangle, fps);
            knightJumpRightAnimation.SetScale(scale);
            knightJumpRightAnimation.SetOffset(spriteOffset);
            Func<bool> isJumpingRight = () => jumpStart > 0f && faceDirection == FaceDirection.RIGHT;
            animations.RegisterStateAnimation("jumpRight", knightJumpRightAnimation, isJumpingRight, 10);

            AnimatedSpriteGroup knightJumpLeftAnimation = new AnimatedSpriteGroup(knightJump, this, spriteBatch, sourceRectangle, fps, SpriteEffects.FlipHorizontally);
            knightJumpLeftAnimation.SetScale(scale);
            knightJumpLeftAnimation.SetOffset(spriteOffset);
            Func<bool> isJumpingLeft = () => jumpStart > 0f && faceDirection == FaceDirection.LEFT;
            animations.RegisterStateAnimation("jumpLeft", knightJumpLeftAnimation, isJumpingLeft, 10);

            //SetSprite(SpriteUtil.CreateRectangle(graphicsDevice, Constants.GRID, Color.Black));

            SetAnimationStateMachime(animations);
        }
    }
}
