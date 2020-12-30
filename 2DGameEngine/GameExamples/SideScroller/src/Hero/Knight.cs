using GameEngine2D.Engine.src.Util;
using GameEngine2D.Entities;
using GameEngine2D.Global;
using GameEngine2D.src;
using GameEngine2D.src.Entities.Animation;
using GameEngine2D.src.Layer;
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
        public Knight(GraphicsDeviceManager graphicsDevice, ContentManager content, SpriteBatch spriteBatch, Vector2 position, SpriteFont font) : base(Scene.Instance.GetEntityLayer(), null, graphicsDevice.GraphicsDevice, position, font)
        {
            List<Texture2D> knightIdle = new List<Texture2D>();
            string folder = "HeroKnight/Idle/";
            for (int i = 0; i <= 7; i++)
            {
                Texture2D t = content.Load<Texture2D>(folder + "HeroKnight_Idle_" + i);
                knightIdle.Add(t);
            }
            //knight = new ControllableEntity(Scene.Instance.GetEntityLayer(), null, graphicsDevice.GraphicsDevice, new Vector2(5, 5) * Constants.GRID, font);
            AnimatedSpriteGroup knightAnimationIdleLeft = new AnimatedSpriteGroup(knightIdle, this, spriteBatch, 30);
            knightAnimationIdleLeft.SetScale(1.5f);
            knightAnimationIdleLeft.SetOffset(new Vector2(70f, 15f) * -1);
            SetIdleAnimationRight(knightAnimationIdleLeft);

            AnimatedSpriteGroup knightAnimationIdleRight = new AnimatedSpriteGroup(knightIdle, this, spriteBatch, 30, SpriteEffects.FlipHorizontally);
            knightAnimationIdleRight.SetScale(1.5f);
            knightAnimationIdleRight.SetOffset(new Vector2(70f, 15f) * -1);
            SetIdleAnimationLeft(knightAnimationIdleRight);

            List<Texture2D> knightRun = new List<Texture2D>();
            folder = "HeroKnight/Run/";
            for (int i = 0; i <= 7; i++)
            {
                Texture2D t = content.Load<Texture2D>(folder + "HeroKnight_Run_" + i);
                knightRun.Add(t);
            }
            AnimatedSpriteGroup knightRunRightAnimation = new AnimatedSpriteGroup(knightRun, this, spriteBatch, 30);
            knightRunRightAnimation.SetScale(1.5f);
            knightRunRightAnimation.SetOffset(new Vector2(70f, 15f) * -1);
            SetMoveRightAnimation(knightRunRightAnimation);

            AnimatedSpriteGroup knightRunLeftAnimation = new AnimatedSpriteGroup(knightRun, this, spriteBatch, 30, SpriteEffects.FlipHorizontally);
            knightRunLeftAnimation.SetScale(1.5f);
            knightRunLeftAnimation.SetOffset(new Vector2(70f, 15f) * -1);
            SetMoveLeftAnimation(knightRunLeftAnimation);

            List<Texture2D> knightJump = new List<Texture2D>();
            folder = "HeroKnight/Jump/";
            for (int i = 0; i <= 2; i++)
            {
                Texture2D t = content.Load<Texture2D>(folder + "HeroKnight_Jump_" + i);
                knightJump.Add(t);
            }
            AnimatedSpriteGroup knightJumpRightAnimation = new AnimatedSpriteGroup(knightJump, this, spriteBatch, 30);
            knightJumpRightAnimation.SetScale(1.5f);
            knightJumpRightAnimation.SetOffset(new Vector2(70f, 15f) * -1);
            SetJumpRightAnimation(knightJumpRightAnimation);

            AnimatedSpriteGroup knightJumpLeftAnimation = new AnimatedSpriteGroup(knightJump, this, spriteBatch, 30, SpriteEffects.FlipHorizontally);
            knightJumpLeftAnimation.SetScale(1.5f);
            knightJumpLeftAnimation.SetOffset(new Vector2(70f, 15f) * -1);
            SetJumpLeftAnimation(knightJumpLeftAnimation);
        }
    }
}
