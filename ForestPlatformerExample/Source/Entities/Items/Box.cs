using ForestPlatformerExample.Source.Enemies;
using ForestPlatformerExample.Source.Entities.Interfaces;
using ForestPlatformerExample.Source.Items;
using ForestPlatformerExample.Source.PlayerCharacter;
using GameEngine2D;
using GameEngine2D.Engine.Source.Entities;
using GameEngine2D.Engine.Source.Entities.Abstract;
using GameEngine2D.Engine.Source.Entities.Animations;
using GameEngine2D.Engine.Source.Physics.Collision;
using GameEngine2D.Engine.Source.Physics.Interface;
using GameEngine2D.Engine.Source.Util;
using GameEngine2D.Entities;
using GameEngine2D.Global;
using GameEngine2D.Source.Entities;
using GameEngine2D.Source.Util;
using GameEngine2D.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForestPlatformerExample.Source.Entities.Items
{
    class Box : AbstractInteractive, IAttackable, IMovableItem
    {

        int life = 2;

        private int bumps;
        private int currentBump = 1;

        private List<Coin> coins = new List<Coin>();

        public Box(Vector2 position, int bumps = 1) : base(position)
        {
            //ColliderOnGrid = true;

            BumpFriction = 0.2f;

            DrawPriority = 1;

            AddCollisionAgainst("Enemy");
            AddTag("Box");

            this.bumps = currentBump = bumps;

            //CollisionComponent = new CircleCollisionComponent(this, 10, new Vector2(0, -8));
            AddComponent(new BoxCollisionComponent(this, 20, 15, new Vector2(-10, -15)));
            //(CollisionComponent as AbstractCollisionComponent).DEBUG_DISPLAY_COLLISION = true;

            CollisionOffsetBottom = 1f;
            CollisionOffsetRight = 0.5f;

            GravityValue /= 2;
            Friction = 0.6f;

            Active = true;

            //DEBUG_SHOW_PIVOT = true;

            AnimationStateMachine Animations = new AnimationStateMachine();
            AddComponent(Animations);
            Animations.Offset = new Vector2(0, -16);

            SpriteSheetAnimation boxIdle = new SpriteSheetAnimation(this, "ForestAssets/Items/box-idle", 24);
            Animations.RegisterAnimation("BoxIdle", boxIdle);

            SpriteSheetAnimation boxHit = new SpriteSheetAnimation(this, "ForestAssets/Items/box-hit", 24);
            boxHit.Looping = false;
            Animations.RegisterAnimation("BoxHit", boxHit);

            SpriteSheetAnimation boxDestroy = new SpriteSheetAnimation(this, "ForestAssets/Items/box-destroy", 24);
            boxDestroy.StartedCallback += () => Pop();
            boxDestroy.Looping = false;
            SetDestroyAnimation(boxDestroy);

            int numOfCoins = MyRandom.Between(3, 6);
            for (int i = 0; i < numOfCoins; i++)
            {

                Coin c = new Coin(Vector2.Zero, 3, friction: (float)MyRandom.Between(87, 93) / (float)100);
                c.BounceCount = 3;
                c.Parent = this;
                c.Visible = false;
                c.CollisionsEnabled = false;
                c.Active = false;
                coins.Add(c);
            }

            //SetSprite(SpriteUtil.CreateRectangle(Config.GRID, Color.Brown));
        }

        public void Hit(Direction impactDireciton)
        {
            if (life == 0)
            {
                Destroy();
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
            Velocity = Vector2.Zero;
            Velocity += force;
            Parent = null;
            EnablePhysics();
            FallSpeed = 0;
        }

        private void EnablePhysics()
        {
            Transform.GridCoordinates = MathUtil.CalculateGridCoordintes(Transform.Position);
            HasGravity = true;
            Active = true;
        }

        private void DisablePysics()
        {
            HasGravity = false;
        }

        private void Pop()
        {
            foreach (Coin c in coins)
            {
                c.Parent = null;
                c.Active = true;
                c.Visible = true;
                c.Velocity += new Vector2(MyRandom.Between(-2, 2), MyRandom.Between(-5, -1));
                Timer.TriggerAfter(500, () => c.CollisionsEnabled = true);
            }
            Layer.Camera.Shake(2f, 0.5f);
            Destroy();
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
            if (otherCollider is Carrot && IsMovingAtLeast(0.5f))
            {
                (otherCollider as Carrot).Destroy();
                Explode();
            }
            base.OnCollisionStart(otherCollider);
        }

        protected override void OnLand()
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
            Velocity = Vector2.Zero;
            GravityValue = 0;
            Destroy();
        }
    }
}
