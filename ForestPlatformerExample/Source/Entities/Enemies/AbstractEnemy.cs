using ForestPlatformerExample.Source.Entities.Interfaces;
using MonolithEngine;
using MonolithEngine.Engine.Source.Entities;
using MonolithEngine.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using MonolithEngine.Engine.Source.Scene;
using MonolithEngine.Engine.Source.Util;

namespace ForestPlatformerExample.Source.Entities.Enemies
{
    abstract class AbstractEnemy : PhysicalEntity, IAttackable
    {
        public int MoveDirection = 1;

        public float DefaultSpeed = 0.05f;

        public float CurrentSpeed = 0.05f;

        protected float RotationRate = 0f;

        protected bool HasDestroyAnimation = false;

        public AbstractEnemy(AbstractScene scene, Vector2 position) : base(scene.LayerManager.EntityLayer, null, position)
        {
            AddTag("Enemy");
        }

        public abstract void Hit(Direction impactDireciton);

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (RotationRate != 0)
            {
                Transform.Rotation += RotationRate;
            }
        }

        public override void Destroy()
        {
            if (!HasDestroyAnimation)
            {
                HorizontalFriction = .99f;
                VerticalFriction = .99f;
                int rand = MyRandom.Between(0, 10);
                Vector2 bump = new Vector2(0.1f, -0.1f);
                RotationRate = 0.1f;
                if (rand % 2 == 0)
                {
                    bump.X *= -1;
                    RotationRate *= -1;
                }
                CheckGridCollisions = false;
                RemoveCollisions();
                Velocity += bump;
                Timer.TriggerAfter(3000, base.Destroy);
            } 
            else
            {
                base.Destroy();
            }
        }
    }
}
