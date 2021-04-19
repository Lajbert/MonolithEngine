using Microsoft.Xna.Framework;
using MonolithEngine;
using MonolithEngine.Engine.Source.Asset;
using MonolithEngine.Engine.Source.Entities;
using MonolithEngine.Engine.Source.Graphics;
using MonolithEngine.Engine.Source.Physics.Collision;
using MonolithEngine.Engine.Source.Scene;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForestPlatformerExample.Source.Entities.Traps
{
    class Saw : PhysicalEntity
    {

        private float ROTATION_RATE = 0.1f;

        private Sprite sprite;

        public Saw(AbstractScene scene, Vector2 position) : base (scene.LayerManager.EntityLayer, null, position)
        {

            AddTag("Saw");

            CanFireTriggers = true;

            sprite = new Sprite(this, Assets.GetTexture("Saw"), new Rectangle(0, 0, 38, 38), origin: new Vector2(19, 19));
            //sprite = new Sprite(this, Assets.GetTexture("Saw"), new Rectangle(0, 0, 38, 38), Vector2.Zero);
            AddComponent(sprite);
            HasGravity = false;

            AddComponent(new CircleCollisionComponent(this, 19));
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            sprite.Rotation += ROTATION_RATE;
        }
    }
}
