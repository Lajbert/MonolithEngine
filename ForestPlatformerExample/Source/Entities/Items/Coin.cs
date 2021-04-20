using ForestPlatformerExample.Source.Entities.Items;
using MonolithEngine;
using MonolithEngine.Engine.Source.Entities;
using MonolithEngine.Engine.Source.Entities.Animations;
using MonolithEngine.Engine.Source.Physics;
using MonolithEngine.Engine.Source.Physics.Collision;
using MonolithEngine.Engine.Source.Physics.Interface;
using MonolithEngine.Engine.Source.Util;
using MonolithEngine.Entities;
using MonolithEngine.Source.Entities;
using MonolithEngine.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using MonolithEngine.Engine.Source.Scene;
using MonolithEngine.Engine.Source.Asset;

namespace ForestPlatformerExample.Source.Items
{
    class Coin : AbstractInteractive
    {
        //private float repelForce = 2;

        public Coin(AbstractScene scene, Vector2 position, int bounceCount = 0, float friction = 0.9f) : base(scene, position)
        {

            Active = true;

            DrawPriority = 2;

            SetCircleCollider();

            HasGravity = true;

            HorizontalFriction = friction;
            VerticalFriction = friction;

            CollisionOffsetBottom = 1;

            //ColliderOnGrid = true;

            //DEBUG_SHOW_PIVOT = true;

            AnimationStateMachine Animations = new AnimationStateMachine();
            Animations.Offset = new Vector2(8, 8);
            AddComponent(Animations);

            SpriteSheetAnimation coinAnim = new SpriteSheetAnimation(this, Assets.GetTexture("CoinPickup"), 30);
            coinAnim.AnimationPauseCondition = () =>
            {
                return !IsAtRest();
            };
            Animations.RegisterAnimation("Idle", coinAnim);

            SpriteSheetAnimation pickupAnim = new SpriteSheetAnimation(this, Assets.GetTexture("CoinPickupEffect"), 45);
            SetDestroyAnimation(pickupAnim);

            //SetSprite(SpriteUtil.CreateRectangle(16, Color.Black));
            //Pivot = new Vector2(5, 5);

#if DEBUG
            //DEBUG_SHOW_COLLIDER = true;
            //DEBUG_SHOW_PIVOT = true;
#endif
        }

        public void SetBump(Vector2 power)
        {
            Bump(power, true);
        }

        public override void PostUpdate()
        {
            if (Destroyed)
            {
                return;
            }
            base.PostUpdate();
            // just a failsafe: in case a coin never bounces for any kind of bug, the player should still be able to pick it up at some point
            if (GetComponent<ICollisionComponent>() == null && Velocity == Vector2.Zero && !BeingDestroyed)
            {
                SetCircleCollider();
            }
        }

        /*public override void OnCollisionStart(IColliderEntity otherCollider)
        {
            if (otherCollider is Coin && repelForce > 0)
            {
                PhysicsUtil.ApplyRepel(this, otherCollider, repelForce, RepelMode.ONLY_THIS);
                repelForce -= 0.5f;
            }
        }*/

        public void SetCircleCollider()
        {
            CircleCollisionComponent collision = new CircleCollisionComponent(this, 10, new Vector2(8, 8));
            AddComponent(collision);
        }

        public override void Update()
        {
            base.Update();
            if (Destroyed)
            {
                return;
            }
            if (Transform.Y > 5000)
            {
                Destroy();
            }
        }
    }
}
