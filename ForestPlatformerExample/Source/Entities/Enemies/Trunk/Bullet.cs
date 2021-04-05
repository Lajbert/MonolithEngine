using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonolithEngine;
using MonolithEngine.Engine.Source.Asset;
using MonolithEngine.Engine.Source.Graphics;
using MonolithEngine.Engine.Source.Physics.Collision;
using MonolithEngine.Engine.Source.Scene;
using MonolithEngine.Global;
using MonolithEngine.Source.Util;
using MonolithEngine.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForestPlatformerExample.Source.Entities.Enemies.Trunk
{
    class Bullet : PhysicalEntity
    {

        public Bullet(AbstractScene scene, Vector2 position, Vector2 speed) : base(scene.LayerManager.EntityLayer, null, position)
        {
            AddTag("Projectile");
            Sprite sprite = new Sprite(this, Assets.GetTexture("TrunkBullet"));
            if (speed.X > 0)
            {
                sprite.SpriteEffect = SpriteEffects.FlipHorizontally;
            }
            AddComponent(sprite);

            CircleCollisionComponent collider = new CircleCollisionComponent(this, 3, new Vector2(8, 8));
            AddComponent(collider);
            CheckGridCollisions = false;
            HorizontalFriction = 0;
            VerticalFriction = 0;
            HasGravity = false;
            Velocity = speed;
            float x = position.X / Config.GRID;
            float y = position.Y / Config.GRID;
            Transform.InCellLocation = new Vector2(x - (float)Math.Truncate(x), y - (float)Math.Truncate(y));
        }
    }
}
