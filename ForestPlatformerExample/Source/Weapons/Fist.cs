using ForestPlatformerExample.Source.Enemies;
using ForestPlatformerExample.Source.Entities.Interfaces;
using GameEngine2D;
using GameEngine2D.Engine.Source.Entities;
using GameEngine2D.Engine.Source.Physics;
using GameEngine2D.Engine.Source.Physics.Collision;
using GameEngine2D.Engine.Source.Physics.Interface;
using GameEngine2D.Engine.Source.Util;
using GameEngine2D.Entities;
using GameEngine2D.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForestPlatformerExample.Source.Weapons
{
    class Fist : PhysicalEntity
    {

        private PhysicalEntity hero;

        public Fist(Entity parent, Vector2 positionOffset) : base(LayerManager.Instance.EntityLayer, parent, positionOffset)
        {
            CircleCollider = new CircleCollisionComponent(this, 10);
            hero = parent as PhysicalEntity;
            DEBUG_SHOW_CIRCLE_COLLIDER = true;
            CurrentFaceDirection = parent.CurrentFaceDirection;

            CollidesAgainst.Add("Enemy");
            CollidesAgainst.Add("Box");
        }

        /*public override void OnCollisionStart(IPhysicsEntity otherCollider)
        {
            Logger.Log("FIST COLLIDES WITH: " + otherCollider);
            if (Timer.IsSet("IsAttacking")
            {
                Direction direction = otherCollider.GetPosition().X < parent.Position.X ? Direction.LEFT : Direction.RIGHT;
                if (otherCollider is IAttackable)
                {
                    (otherCollider as IAttackable).Hit(direction);
                }
            }
        }*/

        /*public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (!Timer.IsSet("IsAttacking") && EnableCircleCollisions)
            {
                EnableCircleCollisions = false;
            } else if (Timer.IsSet("IsAttacking") && !EnableCircleCollisions)
            {
                EnableCircleCollisions = true;
            }
        }*/

        public void Attack()
        {
            if (Timer.IsSet("IsAttacking"))
            {
                return;
            }
            //canAttack = false;
            if (CurrentFaceDirection == Direction.LEFT)
            {
                hero.Animations.PlayAnimation("AttackLeft");
            }
            else if (CurrentFaceDirection == Direction.RIGHT)
            {
                hero.Animations.PlayAnimation("AttackRight");
            }
            if (Timer.IsSet("IsAttacking"))
            {
                return;
            }
            Timer.SetTimer("IsAttacking", 300);
            foreach (IColliderEntity entity in CollisionEngine.Instance.GetCollidesWith(this))
            {
                if (entity is IAttackable)
                {
                    Direction direction = entity.GetPosition().X < parent.Position.X ? Direction.LEFT : Direction.RIGHT;
                    (entity as IAttackable).Hit(direction);
                }
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
