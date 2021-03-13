using ForestPlatformerExample.Source.Entities.Items;
using GameEngine2D;
using GameEngine2D.Engine.Source.Entities.Animations;
using GameEngine2D.Engine.Source.Physics.Collision;
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
    class Spring : AbstractInteractive
    {

        public int Power;
        public Spring(Vector2 position, int power) : base(position)
        {

            Active = true;

            Power = power;

            //CollisionComponent = new CircleCollisionComponent(this, 10, new Vector2(2, 10));
            AddComponent(new BoxCollisionComponent(this, 20, 5, new Vector2(-5, 7)));
            //(CollisionComponent as AbstractCollisionComponent).DEBUG_DISPLAY_COLLISION = true;

            //DEBUG_SHOW_PIVOT = true;

            //DEBUG_SHOW_PIVOT = true;

            AnimationStateMachine Animations = new AnimationStateMachine();
            AddComponent(Animations);
            Animations.Offset = new Vector2(4, 3);

            SpriteSheetAnimation springAnim = new SpriteSheetAnimation(this, "ForestAssets/Items/spring_spritesheet", 24)
            {
                Looping = false
            };
            Animations.RegisterAnimation("Bounce", springAnim);

            //SetSprite(SpriteUtil.CreateRectangle(16, Color.Black));
            //Pivot = new Vector2(5, 5);
        }

        public void PlayBounceAnimation()
        {
            GetComponent<AnimationStateMachine>().PlayAnimation("Bounce");
        }
    }
}
