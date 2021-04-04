using ForestPlatformerExample.Source.PlayerCharacter;
using Microsoft.Xna.Framework;
using MonolithEngine.Engine.Source.Asset;
using MonolithEngine.Engine.Source.Entities;
using MonolithEngine.Engine.Source.Entities.Abstract;
using MonolithEngine.Engine.Source.Entities.Animations;
using MonolithEngine.Engine.Source.Physics.Trigger;
using MonolithEngine.Engine.Source.Scene;
using MonolithEngine.Engine.Source.Util;
using MonolithEngine.Source.Entities;
using MonolithEngine.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForestPlatformerExample.Source.Entities.Enemies.Trunk
{
    class Trunk : AbstractEnemy
    {
        public bool IsAttacking = false;

        private TrunkAIStateMachine AI;

        public Hero Target;

        public Trunk(AbstractScene scene, Vector2 position, Direction currentFaceDirection) : base(scene, position)
        {
            CurrentFaceDirection = currentFaceDirection;

            AnimationStateMachine Animations = new AnimationStateMachine();
            AddComponent(Animations);
            AI = new TrunkAIStateMachine(new TrunkPatrolState(this));
            AI.AddState(new TrunkAttackState(this));
            AddComponent(AI);

#if DEBUG
            AddComponent(new BoxTrigger(512, 256, new Vector2(-256, -128), showTrigger: true));
#else
            AddComponent(new BoxTrigger(256, 256, new Vector2(-256, -128)));
#endif

            /*
            Assets.LoadTexture("TrunkBulletPieces", "ForestAssets/Characters/Trunk/Bullet Pieces");
            Assets.LoadTexture("TrunkBullet", "ForestAssets/Characters/Trunk/Bullet");
            */

            SpriteSheetAnimation idleLeft = new SpriteSheetAnimation(this, Assets.GetTexture("TrunkIdle"), 1, 18, 18, 64, 32, 24);
            Animations.RegisterAnimation("IdleLeft", idleLeft, () => Velocity.X == 0 && CurrentFaceDirection == Direction.WEST);

            SpriteSheetAnimation idleRight = idleLeft.CopyFlipped();
            Animations.RegisterAnimation("IdleRight", idleRight, () => Velocity.X == 0 && CurrentFaceDirection == Direction.EAST);

            SpriteSheetAnimation runLeft = new SpriteSheetAnimation(this, Assets.GetTexture("TrunkRun"), 1, 18, 18, 64, 32, 24);
            Animations.RegisterAnimation("RunLeft", runLeft, () => Velocity.X < 0);

            SpriteSheetAnimation runRight = runLeft.CopyFlipped();
            Animations.RegisterAnimation("RunRight", runRight, () => Velocity.X > 0);

            SpriteSheetAnimation attackLeft = new SpriteSheetAnimation(this, Assets.GetTexture("TrunkAttack"), 1, 11, 11, 64, 32, 24);
            attackLeft.AddFrameAction(8, (frame) => {
                SpawnBullet();
            });
            attackLeft.Looping = false;
            Animations.RegisterAnimation("AttackLeft", attackLeft, () => false);

            SpriteSheetAnimation attackRight = attackLeft.CopyFlipped();
            Animations.RegisterAnimation("AttackRight", attackRight, () => false);

            SpriteSheetAnimation hitLeft = new SpriteSheetAnimation(this, Assets.GetTexture("TrunkHit"), 1, 18, 18, 64, 32, 24);
            Animations.RegisterAnimation("HitLeft", hitLeft, () => false);

            SpriteSheetAnimation hitRight = hitLeft.CopyFlipped();
            Animations.RegisterAnimation("HitRight", hitRight, () => false);

            Active = true;
            Visible = true;
        }

        public override void Hit(Direction impactDireciton)
        {
            
        }

        public void Shoot()
        {
            if (Timer.IsSet("TrunkShooting"))
            {
                return;
            }
            IsAttacking = true;
            Timer.SetTimer("TrunkShooting", 1500);
            Timer.TriggerAfter(1500, () =>
            {
                IsAttacking = false;
            });
            PlayAttackAnimation();
        }

        private void SpawnBullet()
        {
            Vector2 speed = new Vector2(-0.3f, 0);
            if (CurrentFaceDirection == Direction.EAST)
            {
                speed *= -1;
            }

            new Bullet(Scene, Transform.Position - new Vector2(20, 15), speed);
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

    }
}
