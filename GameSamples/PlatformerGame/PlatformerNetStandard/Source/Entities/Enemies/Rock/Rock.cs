using Microsoft.Xna.Framework;
using MonolithEngine;

namespace ForestPlatformerExample
{
    class Rock : AbstractEnemy
    {

        private int health;

        private RockSize size;

        public Rock(AbstractScene scene, Vector2 position, RockSize size = RockSize.BIG, Direction startingFaceDirection = Direction.WEST) : base(scene, position)
        {

            this.size = size;

            CurrentFaceDirection = startingFaceDirection;

            SpriteSheetAnimation runLeft;
            SpriteSheetAnimation runRight;
            SpriteSheetAnimation idleLeft;
            SpriteSheetAnimation idleRight;
            SpriteSheetAnimation hitLeft;
            SpriteSheetAnimation hitRight;

            CircleCollisionComponent collisionComponent;

            CollisionOffsetBottom = 1;

            Vector2 drawOffset;
            Vector2 collisionOffset = new Vector2(0, 2);

            if (size == RockSize.BIG)
            {
                health = 2;

                runLeft = new SpriteSheetAnimation(this, Assets.GetTexture("Rock1Run"), 38, 34, 24);
                runRight = runLeft.CopyFlipped();

                idleLeft = new SpriteSheetAnimation(this, Assets.GetTexture("Rock1Idle"), 38, 34, 24);
                idleRight = idleLeft.CopyFlipped();

                hitLeft = new SpriteSheetAnimation(this, Assets.GetTexture("Rock1Hit"), 38, 34, 24);
                hitRight = hitLeft.CopyFlipped();

                drawOffset = new Vector2(8, -18);

                collisionComponent = new CircleCollisionComponent(this, 15, drawOffset + collisionOffset);
            }
            else if (size == RockSize.MEDIUM)
            {

                health = 1;

                CurrentSpeed = (float)random.Next(8, 11) / 100f;

                runLeft = new SpriteSheetAnimation(this, Assets.GetTexture("Rock2Run"), 32, 28, 24);
                runRight = runLeft.CopyFlipped();

                idleLeft = new SpriteSheetAnimation(this, Assets.GetTexture("Rock2Idle"), 32, 28, 24);
                idleRight = idleLeft.CopyFlipped();

                hitLeft = new SpriteSheetAnimation(this, Assets.GetTexture("Rock2Hit"), 32, 28, 24);
                hitRight = hitLeft.CopyFlipped();

                drawOffset = new Vector2(8, -14);

                collisionComponent = new CircleCollisionComponent(this, 12, drawOffset + collisionOffset);
            }
            else
            {
                health = 0;

                CurrentSpeed = (float)random.Next(12, 16) / 100f;

                runLeft = new SpriteSheetAnimation(this, Assets.GetTexture("Rock3Run"), 22, 18, 24);
                runRight = runLeft.CopyFlipped();

                idleLeft = new SpriteSheetAnimation(this, Assets.GetTexture("Rock3Idle"), 22, 18, 24);
                idleRight = idleLeft.CopyFlipped();

                hitLeft = new SpriteSheetAnimation(this, Assets.GetTexture("Rock3Hit"), 22, 18, 24);
                hitRight = hitLeft.CopyFlipped();

                drawOffset = new Vector2(3, -10);

                collisionComponent = new CircleCollisionComponent(this, 8, drawOffset + collisionOffset);
            }

            bool isRunningLeft() => Transform.VelocityX != 0 && CurrentFaceDirection == Direction.WEST;
            bool isRunningRight() => Transform.VelocityX != 0 && CurrentFaceDirection == Direction.EAST;
            bool isIdleLeft() => Transform.VelocityX == 0 && CurrentFaceDirection == Direction.WEST;
            bool isIdleRight() => Transform.VelocityX == 0 && CurrentFaceDirection == Direction.EAST;

            hitLeft.Looping = false;
            hitRight.Looping = false;

            AnimationStateMachine animations = new AnimationStateMachine();
            animations.Offset = drawOffset;
            animations.RegisterAnimation("RunningLeft", runLeft, isRunningLeft);
            animations.RegisterAnimation("RunningRight", runRight, isRunningRight);
            animations.RegisterAnimation("IdleLeft", idleLeft, isIdleLeft);
            animations.RegisterAnimation("IdleRight", idleRight, isIdleRight);
            animations.RegisterAnimation("HitLeft", hitLeft, () => false);
            animations.RegisterAnimation("HitRight", hitRight, () => false);

            animations.AddFrameTransition("RunningLeft", "RunningRight");
            AddComponent(animations);

            AddComponent(collisionComponent);

            //DEBUG_SHOW_PIVOT = true;
            //DEBUG_SHOW_COLLIDER = true;

        }

        public override void FixedUpdate()
        {
            if (IsOnGround)
            {
                AIUtil.Patrol(true, this);
            }
            base.FixedUpdate();
        }

        public override void Hit(Direction impactDirection)
        {
            if (health == 0)
            {
                if (size == RockSize.BIG)
                {
                    Rock r1 = new Rock(Scene, Transform.Position, RockSize.MEDIUM, Direction.WEST);
                    r1.Transform.Velocity += new Vector2(-1, -0.5f);
                    Rock r2 = new Rock(Scene, Transform.Position, RockSize.MEDIUM, Direction.EAST);
                    r2.Transform.Velocity += new Vector2(1, -0.5f);
                    AudioEngine.Play("RockSplit");
                    Destroy();
                }
                else if (size == RockSize.MEDIUM)
                {
                    Rock r1 = new Rock(Scene, Transform.Position, RockSize.SMALL, Direction.WEST);
                    r1.Transform.Velocity += new Vector2(-1, -0.5f);
                    Rock r2 = new Rock(Scene, Transform.Position, RockSize.SMALL, Direction.EAST);
                    r2.Transform.Velocity += new Vector2(1, -0.5f);
                    AudioEngine.Play("RockSplit");
                    Destroy();
                }
                else
                {
                    AudioEngine.Play("TrunkHit");
                    Transform.Velocity /= 3;
                    CurrentSpeed = DefaultSpeed;
                    Die();
                }

                return;
            }

            health--;

            if (CurrentFaceDirection == Direction.WEST)
            {
                GetComponent<AnimationStateMachine>().PlayAnimation("HitLeft");
            }
            else 
            {
                GetComponent<AnimationStateMachine>().PlayAnimation("HitRight");
            }
            AudioEngine.Play("TrunkHit");
            base.Hit(impactDirection);
        }
    }

    enum RockSize
    {
        BIG,
        MEDIUM,
        SMALL
    }
}
