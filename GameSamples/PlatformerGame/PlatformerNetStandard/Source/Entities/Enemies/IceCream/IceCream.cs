using Microsoft.Xna.Framework;
using MonolithEngine;

namespace ForestPlatformerExample
{
    class IceCream : AbstractEnemy
    {

        public Hero Target;

        private bool isAttacking = false;

        private bool canAttack = true;

        private IceCreamAIStateMachine AI;

        private int health = 2;

        public IceCream(AbstractScene scene, Vector2 position) : base (scene, position)
        {
            DrawPriority = 1;

            AddComponent(new CircleCollisionComponent(this, 12, new Vector2(3, -20)));

            AddComponent(new BoxTrigger(this, 300, 300, new Vector2(-150, -150), "vision"));
            AddTriggeredAgainst(typeof(Hero));

#if DEBUG
            /*DEBUG_SHOW_COLLIDER = true;
            (GetComponent<ITrigger>() as AbstractTrigger).DEBUG_DISPLAY_TRIGGER = true;
            (GetCollisionComponent() as CircleCollisionComponent).DEBUG_DISPLAY_COLLISION = true;
            DEBUG_SHOW_PIVOT = true;*/
#endif

            CurrentFaceDirection = Direction.WEST;

            CollisionOffsetBottom = 1;
            CollisionOffsetLeft = 0.7f;
            CollisionOffsetRight = 0.7f;

            AnimationStateMachine Animations = new AnimationStateMachine();
            AddComponent(Animations);
            Animations.Offset = new Vector2(3, -33);

            SpriteSheetAnimation idleLeft = new SpriteSheetAnimation(this, Assets.GetAnimationTexture("IceCreamIdle"), 23);
            Animations.RegisterAnimation("IdleLeft", idleLeft, () => CurrentFaceDirection == Direction.WEST && Transform.VelocityX == 0);

            SpriteSheetAnimation idleRight = idleLeft.CopyFlipped();
            Animations.RegisterAnimation("IdleRight", idleRight, () => CurrentFaceDirection == Direction.EAST && Transform.VelocityX == 0);

            SpriteSheetAnimation hurtLeft = new SpriteSheetAnimation(this, Assets.GetAnimationTexture("IceCreamHurt"), 24);
            hurtLeft.Looping = false;
            hurtLeft.StartedCallback = () =>
            {
                CurrentSpeed = 0;
                canAttack = false;
            };
            hurtLeft.StoppedCallback = () =>
            {
                CurrentSpeed = DefaultSpeed;
                canAttack = true;
            };
            Animations.RegisterAnimation("HurtLeft", hurtLeft, () => false);

            SpriteSheetAnimation hurtRight = hurtLeft.CopyFlipped();
            Animations.RegisterAnimation("HurtRight", hurtRight, () => false);

            SpriteSheetAnimation moveLeft = new SpriteSheetAnimation(this, Assets.GetAnimationTexture("IceCreamMove"), 17);
            moveLeft.EveryFrameAction = (frame) =>
            {
                if (frame >= 7 && frame <= 10)
                {
                    CurrentSpeed = DefaultSpeed;
                }
                else
                {
                    CurrentSpeed = 0.001f;
                }
            };
            Animations.RegisterAnimation("MoveLeft", moveLeft, () => CurrentFaceDirection == Direction.WEST && Transform.VelocityX != 0);

            SpriteSheetAnimation moveRight = moveLeft.CopyFlipped();
            Animations.RegisterAnimation("MoveRight", moveRight, () => CurrentFaceDirection == Direction.EAST && Transform.VelocityX != 0);

            Animations.AddFrameTransition("MoveLeft", "MoveRight");

            SpriteSheetAnimation attackLeft = new SpriteSheetAnimation(this, Assets.GetAnimationTexture("IceCreamAttack"), 36);
            attackLeft.Looping = false;
            attackLeft.StartedCallback = () => isAttacking = true;
            attackLeft.StoppedCallback = () => isAttacking = false;
            attackLeft.AddFrameAction(21, (frame) =>
            {
                SpawnProjectiles();
            });
            Animations.RegisterAnimation("AttackLeft", attackLeft, () => false);

            SpriteSheetAnimation attackRight = attackLeft.CopyFlipped();
            Animations.RegisterAnimation("AttackRight", attackRight, () => false);

            Animations.AddFrameTransition("AttackLeft", "AttackRight");


            AI = new IceCreamAIStateMachine(new IceCreamPatrolState(this));
            AI.AddState(new IceCreamAttackState(this));

            AddComponent(AI);

            SpriteSheetAnimation deathLeft = new SpriteSheetAnimation(this, Assets.GetAnimationTexture("IceCreamDeath"), 24);
            deathLeft.Looping = false;
            Animations.RegisterAnimation("DeathLeft", deathLeft, () => false);

            SpriteSheetAnimation deathRight = deathLeft.CopyFlipped();
            Animations.RegisterAnimation("DeathRight", deathRight, () => false);
            SetDestroyAnimation(deathLeft, Direction.WEST);
            SetDestroyAnimation(deathRight, Direction.EAST);
        }

        public override void FixedUpdate()
        {
            if (Target != null)
            {
                AI.ChangeState<IceCreamAttackState>();
            }
            else if (!isAttacking)
            {
                AI.ChangeState<IceCreamPatrolState>();
            }
            base.FixedUpdate();
        }

        public void Attack()
        {
            if (isAttacking || !canAttack)
            {
                return;
            }

            if (CurrentFaceDirection == Direction.WEST)
            {
                GetComponent<AnimationStateMachine>().PlayAnimation("AttackLeft");
            }
            else 
            {
                GetComponent<AnimationStateMachine>().PlayAnimation("AttackRight");
            }
        }

        private void SpawnProjectiles()
        {
            AudioEngine.Play("IceCreamExplode");
            IceCreamProjectile p1 = new IceCreamProjectile(Scene, Transform.Position + new Vector2(0, -40));
            p1.AddForce(new Vector2(-0.2f, -0.3f));
            IceCreamProjectile p2 = new IceCreamProjectile(Scene, Transform.Position + new Vector2(0, -40));
            p2.AddForce(new Vector2(0.2f, -0.3f));
        }

        public override void Hit(Direction impactDirection)
        {
            if (health == 0)
            {
                AudioEngine.Play("CarrotExplodeSound");
                CurrentSpeed = 0;
                AI.Enabled = false;
                Die();
                return;
            }

            if (CurrentFaceDirection == Direction.WEST)
            {
                GetComponent<AnimationStateMachine>().PlayAnimation("HurtLeft");
            } 
            else
            {
                GetComponent<AnimationStateMachine>().PlayAnimation("HurtRight");
            }
            health--;
            AudioEngine.Play("TrunkHit");
            base.Hit(impactDirection);
        }

        public override void OnEnterTrigger(string triggerTag, IGameObject otherEntity)
        {
            if (otherEntity is Hero)
            {
                Target = otherEntity as Hero;
            }
            base.OnEnterTrigger(triggerTag, otherEntity);
        }

        public override void OnLeaveTrigger(string triggerTag, IGameObject otherEntity)
        {
            if (otherEntity is Hero)
            {
                Target = null;
            }
            base.OnLeaveTrigger(triggerTag, otherEntity);
        }
    }
}
