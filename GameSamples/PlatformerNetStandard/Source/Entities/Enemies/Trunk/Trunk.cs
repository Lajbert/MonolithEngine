﻿using Microsoft.Xna.Framework;
using MonolithEngine;

namespace ForestPlatformerExample
{
    class Trunk : AbstractEnemy
    {
        public bool IsAttacking = false;

        private bool canAttack = true;

        private TrunkAIStateMachine AI;

        public Hero Target;

        public bool turnedLeft = true;

        private int health = 2;

        public Trunk(AbstractScene scene, Vector2 position, Direction currentFaceDirection) : base(scene, position)
        {
            CurrentFaceDirection = currentFaceDirection;

            DrawPriority = 3;

            AnimationStateMachine Animations = new AnimationStateMachine();
            Animations.Offset = new Vector2(0, -16);
            
            CollisionOffsetBottom = 1;

            AddComponent(Animations);
            AI = new TrunkAIStateMachine(new TrunkPatrolState(this));
            AI.AddState(new TrunkAttackState(this));
            AddComponent(AI);

            AddComponent(new BoxCollisionComponent(this, 20, 20, new Vector2(-10, -24)));
            AddComponent(new BoxTrigger(this, 512, 256, new Vector2(-256, -144), tag: ""));

            /*
            Assets.LoadTexture("TrunkBulletPieces", "ForestAssets/Characters/Trunk/Bullet Pieces");
            Assets.LoadTexture("TrunkBullet", "ForestAssets/Characters/Trunk/Bullet");
            */

            SpriteSheetAnimation idleLeft = new SpriteSheetAnimation(this, Assets.GetTexture("TrunkIdle"), 64, 32, 24);
            idleLeft.Looping = true;
            Animations.RegisterAnimation("IdleLeft", idleLeft, () => Transform.VelocityX == 0 && CurrentFaceDirection == Direction.WEST);

            SpriteSheetAnimation idleRight = idleLeft.CopyFlipped();
            Animations.RegisterAnimation("IdleRight", idleRight, () => Transform.VelocityX == 0 && CurrentFaceDirection == Direction.EAST);

            SpriteSheetAnimation runLeft = new SpriteSheetAnimation(this, Assets.GetTexture("TrunkRun"), 64, 32, 24);
            Animations.RegisterAnimation("RunLeft", runLeft, () => Transform.VelocityX < 0);

            SpriteSheetAnimation runRight = runLeft.CopyFlipped();
            Animations.RegisterAnimation("RunRight", runRight, () => Transform.VelocityX > 0);

            SpriteSheetAnimation attackLeft = new SpriteSheetAnimation(this, Assets.GetTexture("TrunkAttack"),  64, 32, 24);
            attackLeft.AddFrameAction(8, (frame) => {
                SpawnBullet();
            });
            attackLeft.Looping = false;
            attackLeft.StartedCallback = () => turnedLeft = true;
            attackLeft.StoppedCallback = () => turnedLeft = false;
            Animations.RegisterAnimation("AttackLeft", attackLeft, () => false);

            SpriteSheetAnimation attackRight = attackLeft.CopyFlipped();
            attackRight.StartedCallback = () => turnedLeft = false;
            attackRight.StoppedCallback = () => turnedLeft = true;
            Animations.RegisterAnimation("AttackRight", attackRight, () => false);

            SpriteSheetAnimation hitLeft = new SpriteSheetAnimation(this, Assets.GetTexture("TrunkHit"), 64, 32, 24);
            hitLeft.Looping = false;
            hitLeft.StartedCallback = () => canAttack = false;
            hitLeft.StoppedCallback = () => canAttack = true;
            Animations.RegisterAnimation("HitLeft", hitLeft, () => false);

            SpriteSheetAnimation hitRight = hitLeft.CopyFlipped();
            Animations.RegisterAnimation("HitRight", hitRight, () => false);

            Active = true;
            Visible = true;

            /*DebugFunction = () => {
                return AI.GetCurrentState().GetType().Name;
            };*/
        }

        public override void Hit(Direction impactDirection)
        {
            if (health == 0)
            {
                
                Die();
                return;
            }
            AudioEngine.Play("TrunkHit");
            health--;
            PlayHurtAnimation();

            base.Hit(impactDirection);
        }

        public override void Die()
        {
            if (CurrentFaceDirection == Direction.WEST)
            {
                GetComponent<AnimationStateMachine>().PlayAnimation("IdleLeft");
            }
            else
            {
                GetComponent<AnimationStateMachine>().PlayAnimation("IdleRight");
            }
            
            AudioEngine.Play("TrunkDeath");
            base.Die();
        }

        private void PlayHurtAnimation()
        {
            if (CurrentFaceDirection == Direction.WEST)
            {
                GetComponent<AnimationStateMachine>().PlayAnimation("HitLeft");
            }
            else 
            {
                GetComponent<AnimationStateMachine>().PlayAnimation("HitRight");
            }
        }

        public void Shoot()
        {
            if (Timer.IsSet("TrunkShooting" + GetID()) || !canAttack)
            {
                return;
            }

            IsAttacking = true;
            Timer.SetTimer("TrunkShooting" + GetID(), 1500);
            Timer.TriggerAfter(1500, () =>
            {
                IsAttacking = false;
            });
            PlayAttackAnimation();
        }

        private void SpawnBullet()
        {
            AudioEngine.Play("TrunkShoot");
            if (turnedLeft)
            {
                new Bullet(Scene, Transform.Position - new Vector2(29, 15), new Vector2(-0.3f, 0));
            } 
            else
            {
                new Bullet(Scene, Transform.Position + new Vector2(14, -15), new Vector2(0.3f, 0));
            }
        }

        private void PlayAttackAnimation()
        {
            if (CurrentFaceDirection == Direction.WEST)
            {
                GetComponent<AnimationStateMachine>().PlayAnimation("AttackLeft");
            } else
            {
                GetComponent<AnimationStateMachine>().PlayAnimation("AttackRight");
            }
        }

        public override void OnEnterTrigger(string triggerTag, IGameObject otherEntity)
        {
            if (otherEntity is Hero)
            {
                Target = otherEntity as Hero;
                GetComponent<TrunkAIStateMachine>().ChangeState<TrunkAttackState>();
            }
        }

        public override void OnLeaveTrigger(string triggerTag, IGameObject otherEntity)
        {
            if (otherEntity is Hero)
            {
                Target = null;
                GetComponent<TrunkAIStateMachine>().ChangeState<TrunkPatrolState>();
            }
        }

        public override void Update()
        {
            base.Update();
        }

    }
}
