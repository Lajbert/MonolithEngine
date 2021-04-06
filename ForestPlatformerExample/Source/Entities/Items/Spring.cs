using ForestPlatformerExample.Source.Entities.Items;
using MonolithEngine;
using MonolithEngine.Engine.Source.Entities.Animations;
using MonolithEngine.Engine.Source.Physics.Collision;
using MonolithEngine.Engine.Source.Util;
using MonolithEngine.Entities;
using MonolithEngine.Source.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using MonolithEngine.Engine.Source.Scene;
using MonolithEngine.Engine.Source.Asset;
using MonolithEngine.Util;
using MonolithEngine.Engine.Source.Audio;

namespace ForestPlatformerExample.Source.Items
{
    class Spring : AbstractInteractive
    {

        public int Power;
        public Spring(AbstractScene scene, Vector2 position, int power) : base(scene, position)
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

            SpriteSheetAnimation springAnim = new SpriteSheetAnimation(this, Assets.GetTexture("SpringAnim"), 24)
            {
                Looping = false
            };
            Animations.RegisterAnimation("Bounce", springAnim);

            //SetSprite(SpriteUtil.CreateRectangle(16, Color.Black));
            //Pivot = new Vector2(5, 5);
        }

        public void Bounce()
        {
            GetComponent<AnimationStateMachine>().PlayAnimation("Bounce");
            AudioEngine.Play("SpringBounceSound");
        }
    }
}
