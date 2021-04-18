using Microsoft.Xna.Framework;
using MonolithEngine;
using MonolithEngine.Engine.Source.Scene;
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
        }
    }
}
