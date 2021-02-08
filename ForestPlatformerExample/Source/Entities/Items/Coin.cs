using GameEngine2D;
using GameEngine2D.Engine.Source.Entities;
using GameEngine2D.Engine.Source.Entities.Animations;
using GameEngine2D.Engine.Source.Physics.Collision;
using GameEngine2D.Engine.Source.Util;
using GameEngine2D.Entities;
using GameEngine2D.Source.Entities;
using GameEngine2D.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using static GameEngine2D.Engine.Source.Physics.Collision.CollisionResult;

namespace ForestPlatformerExample.Source.Items
{
    class Coin : PhysicalEntity
    {
        private int bounceCount;

        private float repelForce = 2;

        public Coin(Vector2 position, int bounceCount = 0, bool startInactive = false) : base(LayerManager.Instance.EntityLayer, null, position, null)
        {

            AddTag("Pickup");

            this.bounceCount = bounceCount * -1;

            Active = true;

            DrawPriority = 1;

            if (!startInactive)
            {
                SetCircleCollider();
            }

            HasGravity = true;

            Friction = 0.9f;

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
            if (CircleCollider == null)
            {
                SetCircleCollider();
            }
            if (bounceCount <= 0)
            {
                Bump(new Vector2(0, bounceCount++));
            }
            
        }

        public override void PostUpdate(GameTime gameTime)
        {
            base.PostUpdate(gameTime);
            // just a failsafe: in case a coin never bounces for any kind of bug, the player should still be able to pick it up at some point
            if (CircleCollider == null && Velocity == Vector2.Zero)
            {
                SetCircleCollider();
            }
        }

        protected override void OnCircleCollisionStart(Entity otherCollider, CollisionResult collisionResult)
        {
            if (otherCollider is Coin && repelForce > 0)
            {
                collisionResult.ApplyRepel(repelForce, RepelMode.ONLY_THIS);
                repelForce -= 0.5f;
            }
        }

        public void SetCircleCollider()
        {
            CircleCollider = new CircleCollisionComponent(this, 10);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (Destroyed)
            {
                return;
            }
            if (Position.Y > 5000)
            {
                Destroy();
            }
            if (IsMovingAtLeast(0.5f))
            {
                EnableCircleCollisions = true;
            } else
            {
                EnableCircleCollisions = false;
            }
        }
    }
}
