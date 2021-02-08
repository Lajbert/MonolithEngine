using ForestPlatformerExample.Source.Enemies;
using ForestPlatformerExample.Source.Entities.Interfaces;
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
            CircleCollider = new CircleCollisionComponent(this, 10);
            hero = parent as PhysicalEntity;
            //DEBUG_SHOW_CIRCLE_COLLIDER = true;
            CurrentFaceDirection = parent.CurrentFaceDirection;
        }

        protected override void OnCircleCollisionStart(Entity otherCollider, CollisionResult collisionResult)
        {
            if (IsAttacking)
            {
                Direction direction = otherCollider.Position.X < parent.Position.X ? Direction.LEFT : Direction.RIGHT;
                if (otherCollider is IAttackable)
                {
                    (otherCollider as IAttackable).Hit(direction);
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (!IsAttacking && EnableCircleCollisions)
            {
                EnableCircleCollisions = false;
            } else if (IsAttacking && !EnableCircleCollisions)
            {
                EnableCircleCollisions = true;
            }
        }

        public void ChangeDirection()
        {
            if (CurrentFaceDirection != parent.CurrentFaceDirection)
            {
                X = (X - parent.Position.X) * -1;
                CurrentFaceDirection = parent.CurrentFaceDirection;
            }
        }
    }
}
