using Microsoft.Xna.Framework;
using MonolithEngine;
using MonolithEngine.Engine.Source.Asset;
using MonolithEngine.Engine.Source.Graphics;
using MonolithEngine.Engine.Source.Scene;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForestPlatformerExample.Source.Entities.Enemies.Trunk
{
    class Bullet : PhysicalEntity
    {

        public Bullet(AbstractScene scene, Vector2 position, Vector2 speed) : base(scene.LayerManager.EntityLayer, null, position)
        {
            Sprite sprite = new Sprite(this, Assets.GetTexture("TrunkBullet"));
            AddComponent(sprite);

            HorizontalFriction = 0;
            VerticalFriction = 0;
            HasGravity = false;
            Velocity = speed;
        }
    }
}
