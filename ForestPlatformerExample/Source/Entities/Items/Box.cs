using ForestPlatformerExample.Source.Entities.Interfaces;
using GameEngine2D.Engine.Source.Entities;
using GameEngine2D.Engine.Source.Entities.Animations;
using GameEngine2D.Engine.Source.Util;
using GameEngine2D.Entities;
using GameEngine2D.Global;
using GameEngine2D.Source.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForestPlatformerExample.Source.Entities.Items
{
    class Box : Entity, IAttackable
    {

        int life = 2;

        public Box(Vector2 position) : base(LayerManager.Instance.EntityLayer, null, position)
        {
            //ColliderOnGrid = true;

            CircleCollider = new GameEngine2D.Engine.Source.Physics.Collision.CircleCollider(this, 10, new Vector2(0, 3));

            Active = true;

            //DEBUG_SHOW_PIVOT = true;
            //DEBUG_SHOW_CIRCLE_COLLIDER = true;

            Animations = new AnimationStateMachine();
            Animations.Offset = new Vector2(0, -4);

            Texture2D spriteSheet = SpriteUtil.LoadTexture("Green_Greens_Forest_Pixel_Art_Platformer_Pack/Items-and-Objects/Sprite-Sheets/box-idle");
            SpriteSheetAnimation boxIdle = new SpriteSheetAnimation(this, spriteSheet, 2, 7, 13, 32, 32, 24);
            Animations.RegisterAnimation("BoxIdle", boxIdle);

            spriteSheet = SpriteUtil.LoadTexture("Green_Greens_Forest_Pixel_Art_Platformer_Pack/Items-and-Objects/Sprite-Sheets/box-hit");
            SpriteSheetAnimation boxHit = new SpriteSheetAnimation(this, spriteSheet, 1, 5, 5, 32, 32, 24);
            boxHit.Looping = false;
            Animations.RegisterAnimation("BoxHit", boxHit);

            spriteSheet = SpriteUtil.LoadTexture("Green_Greens_Forest_Pixel_Art_Platformer_Pack/Items-and-Objects/Sprite-Sheets/box-destroy");
            SpriteSheetAnimation boxDestroy = new SpriteSheetAnimation(this, spriteSheet, 1, 8, 8, 32, 32, 24);
            boxDestroy.Looping = false;
            SetDestroyAnimation(boxDestroy);
        }

        public void Hit(Direction impactDireciton)
        {
            if (life == 0)
            {
                Destroy();
                return;
            }

            life--;
            Animations.PlayAnimation("BoxHit");
        }
    }
}
