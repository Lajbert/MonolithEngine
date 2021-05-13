using MonolithEngine;
using Microsoft.Xna.Framework;

namespace ForestPlatformerExample
{
    abstract class AbstractEnemy : AbstractDestroyable, IAttackable
    {

        public int MoveDirection = 1;

        public float DefaultSpeed = 0.05f;

        public float CurrentSpeed = 0.05f;

        public AbstractEnemy(AbstractScene scene, Vector2 position) : base(scene, position)
        {
            AddTag("Enemy");
            CurrentFaceDirection = Direction.WEST;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (Transform.Y > 2000 && RotationRate == 0)
            {
                Destroy();
            }

            if (RotationRate != 0)
            {
                Transform.Rotation += RotationRate;
            }
        }

        public virtual void Hit(Direction impactDirection)
        {
            Transform.Velocity = Vector2.Zero;
            Vector2 attackForce = new Vector2(1, -1);
            if (impactDirection == Direction.WEST)
            {
                attackForce.X *= -1;
                Transform.Velocity += attackForce;
            }
            else if (impactDirection == Direction.EAST)
            {
                Transform.Velocity += attackForce;
            }
            FallSpeed = 0;
        }
    }
}
