using ForestPlatformerExample.Source.Enemies;
using ForestPlatformerExample.Source.Entities.Interfaces;
using GameEngine2D;
using GameEngine2D.Engine.Source.Entities;
using GameEngine2D.Engine.Source.Entities.Abstract;
using GameEngine2D.Engine.Source.Entities.Animations;
using GameEngine2D.Engine.Source.Physics;
using GameEngine2D.Engine.Source.Physics.Collision;
using GameEngine2D.Engine.Source.Physics.Interface;
using GameEngine2D.Engine.Source.Util;
using GameEngine2D.Entities;
using GameEngine2D.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ForestPlatformerExample.Source.Weapons
{
    class Fist : PhysicalEntity
    {

        private PhysicalEntity hero;
        private Dictionary<IGameObject, bool> collidesWith = new Dictionary<IGameObject, bool>();

        public Fist(Entity parent, Vector2 positionOffset) : base(LayerManager.Instance.EntityLayer, parent, positionOffset)
        {
            AddComponent(new CircleCollisionComponent(this, 10));
            hero = parent as PhysicalEntity;
            CurrentFaceDirection = parent.CurrentFaceDirection;

            AddCollisionAgainst("Enemy");
            AddCollisionAgainst("Box");

            //DEBUG_SHOW_COLLIDER = true;
        }

        public override void OnCollisionStart(IGameObject otherCollider)
        {
            collidesWith.Add(otherCollider, true);
        }

        public override void OnCollisionEnd(IGameObject otherCollider)
        {
            collidesWith.Remove(otherCollider);
        }

        public void Attack()
        {
            if (Timer.IsSet("IsAttacking"))
            {
                return;
            }
            //canAttack = false;
            if (CurrentFaceDirection == Direction.WEST)
            {
                hero.GetComponent<AnimationStateMachine>().PlayAnimation("AttackLeft");
            }
            else if (CurrentFaceDirection == Direction.EAST)
            {
                hero.GetComponent<AnimationStateMachine>().PlayAnimation("AttackRight");
            }
            Timer.SetTimer("IsAttacking", 300);

            /*foreach (IColliderEntity entity in CollisionEngine.Instance.GetCollidesWith(this))
            {
                if (entity is IAttackable)
                {
                    Direction direction = entity.Transform.X < Parent.Transform.X ? Direction.WEST : Direction.EAST;
                    (entity as IAttackable).Hit(direction);
                }
            }*/

            foreach (IGameObject other in collidesWith.Keys.ToList())
            {
                if (collidesWith[other] && other is IAttackable)
                {
                    Direction direction = other.Transform.X < hero.Transform.X ? Direction.WEST : Direction.EAST;
                    (other as IAttackable).Hit(direction);
                    collidesWith[other] = false;
                }
            }
        }

        public void ChangeDirection()
        {
            if (CurrentFaceDirection != (Parent as Entity).CurrentFaceDirection)
            {
                Transform.X = (Transform.X - Parent.Transform.X) * -1;
                CurrentFaceDirection = (Parent as Entity).CurrentFaceDirection;
            }
        }
    }
}
