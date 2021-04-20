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

        private float ROTATION_RATE = 0.2f;

        private Sprite sprite;

        private float Speed = 0.2f;

        private Vector2 offset = new Vector2(8, 0);

        public Saw(AbstractScene scene, Vector2 position, bool horizontalMovement = true) : base (scene.LayerManager.EntityLayer, null, position)
        {

            AddTag("Saw");

            CanFireTriggers = true;

            CheckGridCollisions = false;

            HorizontalFriction = 0;
            VerticalFriction = 0;

            sprite = new Sprite(this, Assets.GetTexture("Saw"), new Rectangle(0, 0, 38, 38), drawOffset: offset, origin: new Vector2(19, 19));
            AddComponent(sprite);
            HasGravity = false;

            //DEBUG_SHOW_PIVOT = true;

            if (horizontalMovement)
            {
                VelocityX = Speed;
            }
            else
            {
                VelocityY = Speed;
            }

            AddComponent(new CircleCollisionComponent(this, 19, offset));
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            sprite.Rotation += ROTATION_RATE;
        }

        public void ChangeDirection()
        {
            Velocity *= -1;
        }
    }
}
