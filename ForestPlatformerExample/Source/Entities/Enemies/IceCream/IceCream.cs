using Microsoft.Xna.Framework;
using MonolithEngine.Engine.Source.Asset;
using MonolithEngine.Engine.Source.Entities;
using MonolithEngine.Engine.Source.Entities.Animations;
using MonolithEngine.Engine.Source.Physics.Collision;
using MonolithEngine.Engine.Source.Physics.Trigger;
using MonolithEngine.Engine.Source.Scene;
using MonolithEngine.Global;
using MonolithEngine.Source.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForestPlatformerExample.Source.Entities.Enemies.IceCream
{
    class IceCream : AbstractEnemy
    {

        public IceCream(AbstractScene scene, Vector2 position) : base (scene, position)
        {
            DrawPriority = 1;

            AddComponent(new CircleCollisionComponent(this, 12, new Vector2(3, -20)));

            Pivot = new Vector2(Config.GRID / 4, Config.GRID / 4);

            AddComponent(new BoxTrigger(this, 300, 300, new Vector2(-150, -150), "vision"));

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

            SpriteSheetAnimation idleLeft = new SpriteSheetAnimation(this, Assets.GetTexture("IceCreamIdle"), 23);
            Animations.RegisterAnimation("IdleLeft", idleLeft, () => CurrentFaceDirection == Direction.WEST && Velocity.X == 0);

            SpriteSheetAnimation idleRight = idleLeft.CopyFlipped();
            Animations.RegisterAnimation("IdleRight", idleRight, () => CurrentFaceDirection == Direction.EAST && Velocity.X == 0);

            SpriteSheetAnimation hurtLeft = new SpriteSheetAnimation(this, Assets.GetTexture("IceCreamHurt"), 24);
            hurtLeft.Looping = false;
            hurtLeft.StartedCallback = () => CurrentSpeed = 0;
            hurtLeft.StoppedCallback = () => CurrentSpeed = DefaultSpeed;
            Animations.RegisterAnimation("HurtLeft", hurtLeft, () => false);

            SpriteSheetAnimation hurtRight = hurtLeft.CopyFlipped();
            Animations.RegisterAnimation("HurtRight", hurtRight, () => false);

            SpriteSheetAnimation moveLeft = new SpriteSheetAnimation(this, Assets.GetTexture("IceCreamMove"), 17);
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
            Animations.RegisterAnimation("MoveLeft", moveLeft, () => CurrentFaceDirection == Direction.WEST && Velocity.X != 0);

            SpriteSheetAnimation moveRight = moveLeft.CopyFlipped();
            Animations.RegisterAnimation("MoveRight", moveRight, () => CurrentFaceDirection == Direction.EAST && Velocity.X != 0);

            SpriteSheetAnimation deathLeft = new SpriteSheetAnimation(this, Assets.GetTexture("IceCreamDeath"), 24);
            deathLeft.Looping = false;
            Animations.RegisterAnimation("DeathLeft", deathLeft, () => false);

            SpriteSheetAnimation deathRight = deathLeft.CopyFlipped();
            Animations.RegisterAnimation("DeathRight", deathRight, () => false);

            SpriteSheetAnimation attackLeft = new SpriteSheetAnimation(this, Assets.GetTexture("IceCreamDeath"), 24);
            attackLeft.Looping = false;
            Animations.RegisterAnimation("AttackLeft", attackLeft, () => false);

            SpriteSheetAnimation attackRight = attackLeft.CopyFlipped();
            Animations.RegisterAnimation("AttackRight", attackRight, () => false);

            AddComponent(new IceCreamAIStateMachine(new IceCreamPatrolState(this)));
        }

        public override void Hit(Direction impactDireciton)
        {
            if (CurrentFaceDirection == Direction.WEST)
            {
                GetComponent<AnimationStateMachine>().PlayAnimation("HurtLeft");
            } else
            {
                GetComponent<AnimationStateMachine>().PlayAnimation("HurtRight");
            }
        }
    }
}
