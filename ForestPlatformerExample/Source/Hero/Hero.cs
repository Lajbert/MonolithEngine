using GameEngine2D;
using GameEngine2D.Engine.Source.Entities.Animations;
using GameEngine2D.Engine.Source.Util;
using GameEngine2D.Entities;
using GameEngine2D.Source.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForestPlatformerExample.Source.Hero
{
    class Hero : ControllableEntity
    {
        public Hero(Vector2 position, SpriteFont font = null) : base(RootContainer.Instance.EntityLayer, null, position, null, true, font)
        {
            Animations = new AnimationStateMachine();

            Texture2D spiteSheet = SpriteUtil.LoadTexture("Green_Greens_Forest_Pixel_Art_Platformer_Pack/Character-Animations/Main-Character/Sprite-Sheets/main-character@idle-sheet");
            SpriteSheetAnimation idleRight = new SpriteSheetAnimation(this, spiteSheet, 3, 10, 24, 24);
            //knightAnimationIdleRight.Scale = scale;
            //Func<bool> isIdleRight = () => CurrentFaceDirection == Engine.Source.Entities.Direction.RIGHT;
            Func<bool> isIdleRight = () => true;
            Animations.RegisterAnimation("IdleRight", idleRight, isIdleRight);
            //SetSprite(spiteSheet);
        }
    }
}
