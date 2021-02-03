using GameEngine2D;
using GameEngine2D.Engine.Source.Entities;
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
    class Coin : PhysicalEntity
    {
        private int bounceCount;
        public Coin(Vector2 position, int bounceCount = 0) : base(LayerManager.Instance.EntityLayer, null, position, null)
        {

            this.bounceCount = bounceCount * -1;

            Active = true;

            DrawPriority = 1;

            SetCircleCollider();

            HasGravity = false;

            Friction = 0.5f;

            //DEBUG_SHOW_CIRCLE_COLLIDER = true;

            //ColliderOnGrid = true;

            //DEBUG_SHOW_PIVOT = true;

            Animations = new AnimationStateMachine();

            SpriteSheetAnimation coinAnim = new SpriteSheetAnimation(this, "ForestAssets/Items/coin-pickup", 24);
            Animations.RegisterAnimation("Idle", coinAnim);

            SpriteSheetAnimation pickupAnim = new SpriteSheetAnimation(this, "ForestAssets/Items/pickup-effect", 24);
            SetDestroyAnimation(pickupAnim);

            //SetSprite(SpriteUtil.CreateRectangle(16, Color.Black));
            //Pivot = new Vector2(5, 5);
        }

        protected override void OnLand()
        {
            base.OnLand();
            if (bounceCount <= 0)
            {
                Bump(new Vector2(0, bounceCount++));
            }
            
        }

        public void SetCircleCollider()
        {
            CircleCollider = new CircleCollider(this, 10);
        }
    }
}
