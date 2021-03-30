﻿using ForestPlatformerExample.Source.Entities.Items;
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

namespace ForestPlatformerExample.Source.Items
{
    class Coin : AbstractInteractive
    {
        public int BounceCount;

        //private float repelForce = 2;

        public Coin(AbstractScene scene, Vector2 position, int bounceCount = 0, float friction = 0.9f) : base(scene, position)
        {

            Active = true;

            DrawPriority = 1;

            SetCircleCollider();

            HasGravity = true;

            Friction = friction;

            //ColliderOnGrid = true;

            //DEBUG_SHOW_PIVOT = true;

            AnimationStateMachine Animations = new AnimationStateMachine();
            AddComponent(Animations);

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
            if (BounceCount >= 0)
            {
                Bump(new Vector2(0, -BounceCount));
                BounceCount--;
            }
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
            AddComponent(new CircleCollisionComponent(this, 10));
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
