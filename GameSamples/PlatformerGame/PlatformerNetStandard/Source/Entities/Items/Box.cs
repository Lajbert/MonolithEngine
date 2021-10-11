using MonolithEngine;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace ForestPlatformerExample
{
    class Box : AbstractInteractive, IAttackable, IMovableItem
    {

        int life = 2;

        private int bumps;
        private int currentBump = 1;

        private List<Coin> coins = new List<Coin>();

        public Box(AbstractScene scene, Vector2 position, int bumps = 1) : base(scene,  position)
        {
            //ColliderOnGrid = true;

            BumpFriction = 0.2f;

            DrawPriority = 2;

            AddCollisionAgainst(typeof(Carrot));
            AddCollisionAgainst(typeof(Trunk));
            AddCollisionAgainst(typeof(Rock));
            AddCollisionAgainst(typeof(Ghost));
            AddCollisionAgainst(typeof(IceCream));
            AddTag("Box");

            this.bumps = currentBump = bumps;

            //CollisionComponent = new CircleCollisionComponent(this, 10, new Vector2(0, -8));
            AddComponent(new BoxCollisionComponent(this, 20, 15, new Vector2(-10, -15)));
            //(CollisionComponent as AbstractCollisionComponent).DEBUG_DISPLAY_COLLISION = true;

            CollisionOffsetBottom = 1f;
            CollisionOffsetRight = 0.5f;

            GravityValue /= 2;
            HorizontalFriction = 0.8f;

            //DEBUG_SHOW_PIVOT = true;

            AnimationStateMachine Animations = new AnimationStateMachine();
            AddComponent(Animations);
            Animations.Offset = new Vector2(0, -16);

            SpriteSheetAnimation boxIdle = new SpriteSheetAnimation(this, Assets.GetTexture("BoxIdle"), 24);
            Animations.RegisterAnimation("BoxIdle", boxIdle);

            SpriteSheetAnimation boxHit = new SpriteSheetAnimation(this, Assets.GetTexture("BoxHit"), 24)
            {
                Looping = false
            };
            Animations.RegisterAnimation("BoxHit", boxHit);

            SpriteSheetAnimation boxDestroy = new SpriteSheetAnimation(this, Assets.GetTexture("BoxDestroy"), 24);
            boxDestroy.StartedCallback += () => Pop();
            boxDestroy.Looping = false;
            SetDestroyAnimation(boxDestroy);

            int numOfCoins = random.Next(3, 7);
            for (int i = 0; i < numOfCoins; i++)
            {

                Coin c = new Coin(Scene, new Vector2(-8, -20), 3, friction: (float)random.Next(95, 99) / (float)100)
                {
                    Parent = this,
                    Visible = false,
                    CollisionsEnabled = false,
                    Active = false,
                };
                coins.Add(c);
            }

            //SetSprite(SpriteUtil.CreateRectangle(Config.GRID, Color.Brown));
        }

        public void Hit(Direction impactDirection)
        {
            if (life == 0)
            {
                Die();
                return;
            }

            life--;
            GetComponent<AnimationStateMachine>().PlayAnimation("BoxHit");
        }

        public void Lift(Entity entity, Vector2 newPosition)
        {
            currentBump = bumps;
            DisablePysics();
            Parent = entity;
            Transform.Position = newPosition;
        }

        public void PutDown(Entity entity, Vector2 newPosition)
        {
            throw new NotImplementedException();
        }

        public void Throw(Entity entity, Vector2 force)
        {
            Transform.Velocity = Vector2.Zero;
            Transform.Velocity += force;
            Parent = null;
            EnablePhysics();
            FallSpeed = 0;
        }

        private void EnablePhysics()
        {
            HasGravity = true;
            Active = true;
        }

        private void DisablePysics()
        {
            HasGravity = false;
        }

        private void Pop()
        {
            AudioEngine.Play("BoxExplosionSound");
            foreach (Coin c in coins)
            {
                c.Parent = null;
                c.Active = true;
                c.Visible = true;
                float forceMult = 0.003f;
                c.Transform.Velocity += new Vector2(random.Next(-2, 4) * 0.1f, random.Next(-5, 0) * 0.01f);
                c.SetBump(new Vector2(0, -0.5f));
                Timer.TriggerAfter(500, () => c.CollisionsEnabled = true);
            }
            Scene.Camera.Shake();
            Die();
        }

        /*protected override void OnCircleCollisionStart(Entity otherCollider, CollisionResult collisionResult)
        {
            if (otherCollider is Carrot && IsMovingAtLeast(0.5f))
            {
                otherCollider.Destroy();
                Explode();
            }
        }*/

        public override void OnCollisionStart(IGameObject otherCollider)
        {
            if (otherCollider.HasTag("Enemy") && IsMovingAtLeast(0.5f))
            {
                (otherCollider as AbstractEnemy).Die();
                Explode();
            }
            base.OnCollisionStart(otherCollider);
        }

        protected override void OnLand(Vector2 velocity)
        {
            if (currentBump < 1)
            {
                return;
            }
            Bump(new Vector2(0, -currentBump * 2));
            currentBump--;
        }

        private void Explode()
        {
            Transform.Velocity = Vector2.Zero;
            GravityValue = 0;
            Die();
        }
    }
}
