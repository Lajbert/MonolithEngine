using Microsoft.Xna.Framework;
using MonolithEngine.Engine.Source.Asset;
using MonolithEngine.Engine.Source.Entities;
using MonolithEngine.Engine.Source.Entities.Animations;
using MonolithEngine.Engine.Source.Physics.Collision;
using MonolithEngine.Engine.Source.Scene;
using MonolithEngine.Engine.Source.Util;
using MonolithEngine.Source.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForestPlatformerExample.Source.Entities.Enemies.Ghost
{
    class Ghost : AbstractEnemy
    {
        private readonly int APPEAR_DISAPPEAR_TIMEOUT = 2000;

        private bool beingHit = false;

        public Ghost(AbstractScene scene, Vector2 position) : base (scene, position)
        {
            AnimationStateMachine animations = new AnimationStateMachine();
            AddComponent(animations);

            SpriteSheetAnimation appearLeft = new SpriteSheetAnimation(this, Assets.GetTexture("GhostAppear"), 44, 30, 24);
            appearLeft.Looping = false;
            appearLeft.StartedCallback = () =>
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

            Timer.TriggerAfter(APPEAR_DISAPPEAR_TIMEOUT, Disappear);
        }

        private void Appear()
        {
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
            if (RotationRate != 0 || beingHit)
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

        public override void Hit(Direction impactDireciton)
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
