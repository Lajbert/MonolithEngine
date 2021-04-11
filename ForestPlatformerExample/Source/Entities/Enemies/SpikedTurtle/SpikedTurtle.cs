using Microsoft.Xna.Framework;
using MonolithEngine.Engine.Source.Asset;
using MonolithEngine.Engine.Source.Audio;
using MonolithEngine.Engine.Source.Entities;
using MonolithEngine.Engine.Source.Entities.Animations;
using MonolithEngine.Engine.Source.Physics.Collision;
using MonolithEngine.Engine.Source.Scene;
using MonolithEngine.Engine.Source.Util;
using MonolithEngine.Source.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForestPlatformerExample.Source.Entities.Enemies.SpikedTurtle
{
    class SpikedTurtle : AbstractEnemy
    {

        public bool SpikesOut = false;

        private float health = 2;

        private bool hurt = false;

        public SpikedTurtle(AbstractScene scene, Vector2 position, Direction currentFaceDirection) : base(scene, position)
        {

            CurrentFaceDirection = currentFaceDirection;

            AddComponent(new CircleCollisionComponent(this, 17, new Vector2(0, -5)));
            AnimationStateMachine Animations = new AnimationStateMachine();
            AddComponent(Animations);
            Animations.Offset = new Vector2(0, -13);
            CollisionOffsetBottom = 1;

            SpriteSheetAnimation idleNormalLeft = new SpriteSheetAnimation(this, Assets.GetTexture("TurtleIdleNormal"), 1, 14, 14, 44, 26, 24);
            idleNormalLeft.Looping = true;
            idleNormalLeft.StartedCallback = () =>
            {
                Timer.TriggerAfter(3000, () =>
                {
                    SpikesOutState();
                });
            };

            Animations.RegisterAnimation("IdleNormalLeft", idleNormalLeft, () => CurrentFaceDirection == Direction.WEST);

            SpriteSheetAnimation idleNormalRight = idleNormalLeft.CopyFlipped();
            Animations.RegisterAnimation("IdleNormalRight", idleNormalRight, () => CurrentFaceDirection == Direction.EAST);

            SpriteSheetAnimation idleSpikedLeft = new SpriteSheetAnimation(this, Assets.GetTexture("TurtleIdleSpiked"), 1, 14, 14, 44, 26, 24);
            idleSpikedLeft.Looping = true;
            Animations.RegisterAnimation("IdleSpikedLeft", idleSpikedLeft, () => CurrentFaceDirection == Direction.WEST);

            SpriteSheetAnimation idleSpikedRight = idleSpikedLeft.CopyFlipped();
            Animations.RegisterAnimation("IdleSpikedRight", idleSpikedRight, () => CurrentFaceDirection == Direction.EAST);

            SpriteSheetAnimation hitLeft = new SpriteSheetAnimation(this, Assets.GetTexture("TurtleHit"), 1, 5, 5, 44, 26, 24);
            hitLeft.Looping = false;
            hitLeft.StartedCallback = () => { hurt = true; };
            hitLeft.StoppedCallback = () => { hurt = false; };
            Animations.RegisterAnimation("HitLeft", hitLeft, () => CurrentFaceDirection == Direction.WEST);

            SpriteSheetAnimation hitRight = hitLeft.CopyFlipped();
            Animations.RegisterAnimation("HitRight", hitRight, () => CurrentFaceDirection == Direction.EAST);

            SpriteSheetAnimation spikesOutLeft = new SpriteSheetAnimation(this, Assets.GetTexture("TurtleSpikesOut"), 1, 8, 8, 44, 26, 60);
            spikesOutLeft.Looping = false;
            spikesOutLeft.AddFrameAction(5, (frame) =>
            {
                SpikesOut = true;
            });
            spikesOutLeft.StoppedCallback = () =>
            {
                SpikesOutWait();
            };
            Animations.RegisterAnimation("SpikesOutLeft", spikesOutLeft, () => false);

            SpriteSheetAnimation spikesOutRight = spikesOutLeft.CopyFlipped();
            Animations.RegisterAnimation("SpikesOutRight", spikesOutRight, () => false);

            SpriteSheetAnimation spikesInLeft = new SpriteSheetAnimation(this, Assets.GetTexture("TurtleSpikesIn"), 1, 8, 8, 44, 26, 60);
            spikesInLeft.Looping = false;
            spikesInLeft.AddFrameAction(6, (frame) =>
            {
                SpikesOut = false;
            });
            Animations.RegisterAnimation("SpikesInLeft", spikesInLeft, () => CurrentFaceDirection == Direction.WEST);

            SpriteSheetAnimation spikesInRight = spikesInLeft.CopyFlipped();
            Animations.RegisterAnimation("SpikesInRight", spikesInRight, () => CurrentFaceDirection == Direction.EAST);


#if DEBUG
            //DEBUG_SHOW_PIVOT = true;
            //DEBUG_SHOW_COLLIDER = true;
#endif
        }

        private void SpikesOutState()
        {
            if (hurt)
            {
                return;
            }
            if (CurrentFaceDirection == Direction.WEST)
            {
                GetComponent<AnimationStateMachine>().PlayAnimation("SpikesOutLeft");
            }
            else
            {
                GetComponent<AnimationStateMachine>().PlayAnimation("SpikesOutRight");
            }
        }

        private void SpikesOutWait()
        {
            string nextAnim = "SpikesInLeft";
            if (CurrentFaceDirection == Direction.WEST)
            {
                GetComponent<AnimationStateMachine>().PlayAnimation("IdleSpikedLeft");
            }
            else
            {
                GetComponent<AnimationStateMachine>().PlayAnimation("IdleSpikedRight");
                nextAnim = "SpikesInRight";
            }
            Timer.TriggerAfter(3000, () => { 
                if (RotationRate == 0) PlayNext(nextAnim); 
            });
        }

        private void PlayNext(string nextAnim)
        {
            GetComponent<AnimationStateMachine>().PlayAnimation(nextAnim);
            SpikesOut = true;
        }

        public override void Hit(Direction impactDireciton)
        {
            if (health == 0)
            {

                Destroy();
                return;
            }
            //AudioEngine.Play("TrunkHit");
            health--;
            PlayHurtAnimation();
        }

        public override void Destroy()
        {
            if (CurrentFaceDirection == Direction.WEST)
            {
                GetComponent<AnimationStateMachine>().PlayAnimation("IdleNormalLeft");
            }
            else
            {
                GetComponent<AnimationStateMachine>().PlayAnimation("IdleNormalRight");
            }

            //AudioEngine.Play("TrunkDeath");
            base.Destroy();
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
    }
}
