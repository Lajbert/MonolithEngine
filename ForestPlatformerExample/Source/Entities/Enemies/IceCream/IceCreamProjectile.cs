using Microsoft.Xna.Framework;
using MonolithEngine;
using MonolithEngine.Engine.Source.Asset;
using MonolithEngine.Engine.Source.Entities.Animations;
using MonolithEngine.Engine.Source.Physics.Collision;
using MonolithEngine.Engine.Source.Scene;
using MonolithEngine.Engine.Source.Util;
using MonolithEngine.Global;
using MonolithEngine.Source.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForestPlatformerExample.Source.Entities.Enemies.IceCream
{
    class IceCreamProjectile : PhysicalEntity
    {
        public IceCreamProjectile(AbstractScene scene, Vector2 position) : base(scene.LayerManager.EntityLayer, null, position)
        {
            AddTag("IceCreamProjectile");
            CheckGridCollisions = true;

            GravityValue /= 3;
            DrawPriority = 0;

            HorizontalFriction = 0.99f;
            VerticalFriction = 0.99f;

            AnimationStateMachine Animations = new AnimationStateMachine();
            AddComponent(Animations);
            SpriteSheetAnimation idle = new SpriteSheetAnimation(this, Assets.GetTexture("IceCreamProjectileIdle"), 1, 6, 6, 11, 11, 24);
            Animations.RegisterAnimation("Idle", idle);

            /*SpriteSheetAnimation hit = new SpriteSheetAnimation(this, Assets.GetTexture("IceCreamProjectileHit"), 24);
            Animations.RegisterAnimation("Hit", hit, () => false);*/

            //Timer.TriggerAfter(5000, Destroy);
        }
    }
}
