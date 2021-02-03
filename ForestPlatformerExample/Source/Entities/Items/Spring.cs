using GameEngine2D.Engine.Source.Entities.Animations;
using GameEngine2D.Engine.Source.Util;
using GameEngine2D.Entities;
using GameEngine2D.Source.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForestPlatformerExample.Source.Items
{
    class Spring : Entity
    {

        public int Power;
        public Spring(Vector2 position, int power) : base(LayerManager.Instance.EntityLayer, null, position, null)
        {

            Active = true;

            Power = power;

            ColliderOnGrid = true;

            //DEBUG_SHOW_PIVOT = true;

            Animations = new AnimationStateMachine();
            Animations.Offset = new Vector2(4, 3);

            SpriteSheetAnimation springAnim = new SpriteSheetAnimation(this, "ForestAssets/Items/spring_spritesheet", 24);
            springAnim.Looping = false;
            Animations.RegisterAnimation("Bounce", springAnim);

            //SetSprite(SpriteUtil.CreateRectangle(16, Color.Black));
            //Pivot = new Vector2(5, 5);
        }

        public void PlayBounceAnimation()
        {
            Animations.PlayAnimation("Bounce");
        }
    }
}
