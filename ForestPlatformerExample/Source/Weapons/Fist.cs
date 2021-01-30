using ForestPlatformerExample.Source.Enemies;
using GameEngine2D;
using GameEngine2D.Engine.Source.Entities;
using GameEngine2D.Engine.Source.Physics.Collision;
using GameEngine2D.Engine.Source.Util;
using GameEngine2D.Entities;
using GameEngine2D.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForestPlatformerExample.Source.Weapons
{
    class Fist : Entity
    {

        public bool IsAttacking = false;
        private PhysicalEntity hero;

        public Fist(Entity parent, Vector2 positionOffset) : base(LayerManager.Instance.EntityLayer, parent, positionOffset)
        {
            CircleCollider = new CircleCollider(this, 10, positionOffset);
            GridCollisionCheckDirections = new HashSet<Direction> { Direction.CENTER };
            hero = parent as PhysicalEntity;
            //DEBUG_SHOW_CIRCLE_COLLIDER = true;
        }

        protected override void OnCircleCollision(Entity otherCollider, float intersection)
        {
            if (IsAttacking)
            {
                if (otherCollider is Carrot)
                {
                    if (Timer.IsSet("EnemyHit"))
                    {
                        return;
                    }
                    Direction direction = otherCollider.Position.X < parent.Position.X ? Direction.LEFT : Direction.RIGHT;
                    (otherCollider as Carrot).Punch(direction);
                    //Timer.SetTimer("EnemyHit", 1000);
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (hero.CurrentFaceDirection == Direction.LEFT)
            {
                X = -3 * StartPosition.X; 
            } else
            {
                X = StartPosition.X;
            }
        }

        public void ChangeDirection()
        {
            X *= -1;
        }
    }
}
