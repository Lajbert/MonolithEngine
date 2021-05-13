using Microsoft.Xna.Framework;
using MonolithEngine;

namespace ForestPlatformerExample
{
    class Ghost : AbstractEnemy
    {
        private readonly int APPEAR_DISAPPEAR_TIMEOUT = 2000;

        private bool beingHit = false;

        private int health = 2;

        public Ghost(AbstractScene scene, Vector2 position) : base (scene, position)
        {
            AnimationStateMachine animations = new AnimationStateMachine();
            AddComponent(animations);

            SpriteSheetAnimation appearLeft = new SpriteSheetAnimation(this, Assets.GetTexture("GhostAppear"), 44, 30, 24);
            appearLeft.Looping = false;
            appearLeft.StoppedCallback = () =>
            {
                CollisionsEnabled = true;
            };
            animations.RegisterAnimation("AppearLeft", appearLeft, () => false);
            SpriteSheetAnimation appearRight = appearLeft.CopyFlipped();
            animations.RegisterAnimation("AppearRight", appearRight, () => false);

            SpriteSheetAnimation disappearLeft = new SpriteSheetAnimation(this, Assets.GetTexture("GhostDisappear"), 44, 30, 24);
            disappearLeft.Looping = false;
            disappearLeft.StoppedCallback = () =>
            {
                CollisionsEnabled = false;
                Visible = false;
            };
            animations.RegisterAnimation("DisappearLeft", disappearLeft, () => false);
            SpriteSheetAnimation disappearRight = disappearLeft.CopyFlipped();
            animations.RegisterAnimation("DisappearRight", disappearRight, () => false);

            SpriteSheetAnimation hitLeft = new SpriteSheetAnimation(this, Assets.GetTexture("GhostHit"), 44, 30, 24);
            hitLeft.Looping = false;
            hitLeft.StartedCallback = () =>
            {
                beingHit = true;
            };
            hitLeft.StoppedCallback = () =>
            {
                beingHit = false;
                Timer.TriggerAfter(APPEAR_DISAPPEAR_TIMEOUT, Disappear);
            };
            animations.RegisterAnimation("HitLeft", hitLeft, () => false);
            SpriteSheetAnimation hitRight = hitLeft.CopyFlipped();
            animations.RegisterAnimation("HitRight", hitRight, () => false);

            SpriteSheetAnimation idleLeft = new SpriteSheetAnimation(this, Assets.GetTexture("GhostIdle"), 44, 30, 24);
            animations.RegisterAnimation("IdleLeft", idleLeft, () => CurrentFaceDirection == Direction.WEST);
            SpriteSheetAnimation idleRight = idleLeft.CopyFlipped();
            animations.RegisterAnimation("IdleRight", idleRight, () => CurrentFaceDirection == Direction.EAST);

            animations.AddFrameTransition("IdleLeft", "IdleRight");

            AddComponent(new BoxCollisionComponent(this, 25, 25, new Vector2(-12, -12)));

            Timer.TriggerAfter(random.Next(0, APPEAR_DISAPPEAR_TIMEOUT), Disappear);
        }

        private void Appear()
        {
            //AudioEngine.Play("GostAppear");
            if (RotationRate != 0 || Destroyed || BeingDestroyed)
            {
                return;
            }
            Visible = true;
            if (CurrentFaceDirection == Direction.WEST)
            {
                GetComponent<AnimationStateMachine>().PlayAnimation("AppearLeft");
            }
            else
            {
                GetComponent<AnimationStateMachine>().PlayAnimation("AppearRight");
            }

            Timer.TriggerAfter(APPEAR_DISAPPEAR_TIMEOUT, Disappear);
        }

        private void Disappear()
        {
            //AudioEngine.Play("GostDisappear");
            if (RotationRate != 0 || beingHit || Destroyed || BeingDestroyed)
            {
                return;
            }
            if (CurrentFaceDirection == Direction.WEST)
            {
                GetComponent<AnimationStateMachine>().PlayAnimation("DisappearLeft");
            }
            else
            {
                GetComponent<AnimationStateMachine>().PlayAnimation("DisappearRight");
            }
            Timer.TriggerAfter(APPEAR_DISAPPEAR_TIMEOUT, Appear);
        }

        public override void Hit(Direction impactDirection)
        {
            AudioEngine.Play("TrunkHit");
            if (health == 0)
            {
                Die();
                return;
            }

            if (CurrentFaceDirection == Direction.WEST)
            {
                GetComponent<AnimationStateMachine>().PlayAnimation("HitLeft");
            }
            else
            {
                GetComponent<AnimationStateMachine>().PlayAnimation("HitRight");
            }

            health--;

            base.Hit(impactDirection);
        }

        public override void FixedUpdate()
        {
            if (IsOnGround && !beingHit)
            {
                AIUtil.Patrol(true, this);
            }
            base.FixedUpdate();
        }
    }
}
