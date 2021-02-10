using ForestPlatformerExample.Source.Entities.Interfaces;
using GameEngine2D;
using GameEngine2D.Engine.Source.Entities;
using GameEngine2D.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForestPlatformerExample.Source.Entities.Enemies
{
    abstract class AbstractEnemy : PhysicalEntity, IAttackable
    {

        public AbstractEnemy(Vector2 position) : base(LayerManager.Instance.EntityLayer, null, position)
        {
            AddTag("Enemy");
        }

        public abstract void Hit(Direction impactDireciton);
    }
}
