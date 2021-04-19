using Microsoft.Xna.Framework;
using MonolithEngine;
using MonolithEngine.Engine.Source.Asset;
using MonolithEngine.Engine.Source.Entities.Animations;
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
            /*
            Assets.LoadTexture("IceCreamProjectileHit", "IcySkies/Characters/IceCream/ice-cream-projectile@hit");
            Assets.LoadTexture("IceCreamProjectileIdle", "IcySkies/Characters/IceCream/ice-cream-projectile@idle");
             */

            GravityValue /= 3;
            DrawPriority = 0;

            CheckGridCollisions = false;

            HorizontalFriction = 0.99f;
            VerticalFriction = 0.99f;

            AnimationStateMachine Animations = new AnimationStateMachine();
            AddComponent(Animations);
            //Entity parent, Texture2D texture, int rows, int columns, int totalFrames, int width = 0, int height = 0, int framerate = 0, SpriteEffects spriteEffect = SpriteEffects.None
            //SpriteSheetAnimation idle = new SpriteSheetAnimation(this, Assets.GetTexture("IceCreamProjectileIdle"), 24);
            SpriteSheetAnimation idle = new SpriteSheetAnimation(this, Assets.GetTexture("IceCreamProjectileIdle"), 1, 6, 6, 11, 11, 24);
            Animations.RegisterAnimation("Idle", idle);

            /*SpriteSheetAnimation hit = new SpriteSheetAnimation(this, Assets.GetTexture("IceCreamProjectileHit"), 24);
            Animations.RegisterAnimation("Hit", hit, () => false);*/

            Timer.TriggerAfter(5000, Destroy);
        }
    }
}
