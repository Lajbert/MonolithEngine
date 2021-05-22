using MonolithEngine;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace ForestPlatformerExample
{
    class Fist : PhysicalEntity
    {

        private PhysicalEntity hero;
        private List<IGameObject> collidesWith = new List<IGameObject>();

        public Fist(AbstractScene scene, Entity parent, Vector2 positionOffset) : base(scene.LayerManager.EntityLayer, parent, positionOffset)
        {
            AddComponent(new CircleCollisionComponent(this, 10));
            CheckGridCollisions = false;
            HasGravity = false;
            hero = parent as PhysicalEntity;
            CurrentFaceDirection = parent.CurrentFaceDirection;

            AddCollisionAgainst("Enemy");
            AddCollisionAgainst("Box");

            //DEBUG_SHOW_COLLIDER = true;
        }

        public override void OnCollisionStart(IGameObject otherCollider)
        {
            collidesWith.Add(otherCollider);
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

            AudioEngine.Play("HeroPunch");
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

            foreach (IGameObject other in collidesWith)
            {
                if (other is IAttackable)
                {
                    Direction direction = other.Transform.X < hero.Transform.X ? Direction.WEST : Direction.EAST;
                    (other as IAttackable).Hit(direction);
                }
            }
        }

        public void ChangeDirection()
        {
            if (CurrentFaceDirection != (Parent as Entity).CurrentFaceDirection)
            {
                CurrentFaceDirection = (Parent as Entity).CurrentFaceDirection;
                int mul = 1;
                if (CurrentFaceDirection == Direction.WEST)
                {
                    mul = -1;
                }
                Transform.Position = new Vector2(mul * 20, -10);
            }
        }
    }
}
