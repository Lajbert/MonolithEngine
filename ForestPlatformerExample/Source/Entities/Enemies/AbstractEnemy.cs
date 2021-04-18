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
using MonolithEngine.Engine.Source.Entities.Animations;
using MonolithEngine.Source.Entities.Animation;

namespace ForestPlatformerExample.Source.Entities.Enemies
{
    abstract class AbstractEnemy : AbstractDestroyable, IAttackable
    {

        public int MoveDirection = 1;

        public float DefaultSpeed = 0.05f;

        public float CurrentSpeed = 0.05f;

        public AbstractEnemy(AbstractScene scene, Vector2 position) : base(scene, position)
        {
            AddTag("Enemy");
        }

        public abstract void Hit(Direction impactDireciton);

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (Transform.Y > 2000 && RotationRate == 0)
            {
                Destroy();
            }

            if (RotationRate != 0)
            {
                Transform.Rotation += RotationRate;
            }
        }
    }
}
