using ForestPlatformerExample.Source.Entities.Interfaces;
using MonolithEngine;
using MonolithEngine.Engine.Source.Entities;
using MonolithEngine.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using MonolithEngine.Engine.Source.Scene;

namespace ForestPlatformerExample.Source.Entities.Enemies
{
    abstract class AbstractEnemy : PhysicalEntity, IAttackable
    {
        public int MoveDirection = 1;

        public float DefaultSpeed = 0.05f;

        public float CurrentSpeed = 0.05f;

        public AbstractEnemy(AbstractScene scene, Vector2 position) : base(scene.LayerManager.EntityLayer, null, position)
        {
            AddTag("Enemy");
        }

        public abstract void Hit(Direction impactDireciton);
    }
}
