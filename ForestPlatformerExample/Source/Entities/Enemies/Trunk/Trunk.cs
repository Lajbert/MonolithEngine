using Microsoft.Xna.Framework;
using MonolithEngine.Engine.Source.Asset;
using MonolithEngine.Engine.Source.Entities;
using MonolithEngine.Engine.Source.Entities.Animations;
using MonolithEngine.Engine.Source.Scene;
using MonolithEngine.Source.Entities;
using MonolithEngine.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForestPlatformerExample.Source.Entities.Enemies.Trunk
{
    class Trunk : AbstractEnemy
    {
        private bool isAttacking = false;

        private TrunkAIStateMachine AI;

        public Trunk(AbstractScene scene, Vector2 position, Direction currentFaceDirection) : base(scene, position)
        {
            CurrentFaceDirection = currentFaceDirection;

            AnimationStateMachine Animations = new AnimationStateMachine();
            AddComponent(Animations);
            AI = new TrunkAIStateMachine(new TrunkPatrolState(this));
            AddComponent(AI);

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
            Animations.RegisterAnimation("AttackLeft", attackLeft, () => isAttacking && CurrentFaceDirection == Direction.WEST);

            SpriteSheetAnimation attackRight = attackLeft.CopyFlipped();
            Animations.RegisterAnimation("AttackRight", attackRight, () => isAttacking && CurrentFaceDirection == Direction.EAST);

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

    }
}
