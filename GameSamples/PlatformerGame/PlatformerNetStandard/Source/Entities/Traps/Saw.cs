﻿using Microsoft.Xna.Framework;
using MonolithEngine;

namespace ForestPlatformerExample
{
    class Saw : PhysicalEntity
    {

        private float ROTATION_RATE = 0.2f;

        private Sprite sprite;

        private float Speed = 0.2f;

        //private Vector2 offset = new Vector2(8, 0);

        public Saw(AbstractScene scene, Vector2 position, bool horizontalMovement = true, Vector2 pivot = default) : base (scene.LayerManager.EntityLayer, null, position)
        {

            AddTag("Saw");

            CanFireTriggers = true;

            //CollisionOffsetBottom = 1;

            CheckGridCollisions = false;

            HorizontalFriction = 0;
            VerticalFriction = 0;

            Pivot = pivot;

            sprite = new Sprite(this, Assets.GetTexture("Saw"));
            sprite.Origin = new Vector2(sprite.SourceRectangle.Width / 2, sprite.SourceRectangle.Height / 2);
            AddComponent(sprite);
            HasGravity = false;

            //DEBUG_SHOW_PIVOT = true;

            if (horizontalMovement)
            {
                Transform.VelocityX = Speed;
            }
            else
            {
                Transform.VelocityY = Speed;
            }

            AddComponent(new CircleCollisionComponent(this, 19, new Vector2(sprite.SourceRectangle.Width, sprite.SourceRectangle.Height) * -Pivot));
#if DEBUG
            DEBUG_SHOW_COLLIDER = true;
#endif
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            sprite.Rotation += ROTATION_RATE;
        }

        public void ChangeDirection()
        {
            Transform.Velocity *= -1;
        }
    }
}
