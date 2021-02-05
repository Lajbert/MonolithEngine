using ForestPlatformerExample.Source.Enemies;
using ForestPlatformerExample.Source.Entities.Interfaces;
using ForestPlatformerExample.Source.Items;
using ForestPlatformerExample.Source.PlayerCharacter;
using GameEngine2D;
using GameEngine2D.Engine.Source.Entities;
using GameEngine2D.Engine.Source.Entities.Animations;
using GameEngine2D.Engine.Source.Physics.Collision;
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
    class Box : PhysicalEntity, IAttackable, IMovableItem
    {

        int life = 2;

        private Random random;
        private int bumps;
        private int currentBump = 1;

        public Box(Vector2 position, int bumps = 1) : base(LayerManager.Instance.EntityLayer, null, position)
        {
            //ColliderOnGrid = true;

            BumpFriction = 0.2f;

            DrawPriority = 1;

            this.bumps = currentBump = bumps;

            random = new Random();

            CircleCollider = new CircleCollider(this, 10, new Vector2(0, 0));
            EnableCircleCollisions = false;

            CollisionOffsetBottom = 1f;
            CollisionOffsetRight = 0.5f;

            GravityValue /= 2;
            Friction = 0.6f;

            Active = true;

            //DEBUG_SHOW_PIVOT = true;
            //DEBUG_SHOW_CIRCLE_COLLIDER = true;

            Animations = new AnimationStateMachine();
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
            Animations.PlayAnimation("BoxHit");
        }

        public void Lift(Entity entity, Vector2 newPosition)
        {
            currentBump = bumps;
            DisablePysics();
            SetParent(entity, newPosition);
        }

        public void PutDown(Entity entity, Vector2 newPosition)
        {
            throw new NotImplementedException();
        }

        public void Throw(Entity entity, Vector2 force)
        {
            Velocity = Vector2.Zero;
            Velocity += force;
            RemoveParent();
            EnablePhysics();
            FallSpeed = 0;
        }

        private void EnablePhysics()
        {
            GridCoordinates = CalculateGridCoord();
            UpdateInCellCoord();
            GridCollisionCheckDirections = new HashSet<Direction>() { Direction.UP, Direction.DOWN, Direction.LEFT, Direction.RIGHT };
            HasGravity = true;
            Active = true;
            EnableCircleCollisions = true;
        }

        private void DisablePysics()
        {
            EnableCircleCollisions = false;
            GridCollisionCheckDirections = new HashSet<Direction>();
            HasGravity = false;
        }

        private void Pop()
        {
            int numOfCoins = random.Next(3, 6);
            for (int i = 0; i < numOfCoins; i++)
            {
                Coin c = new Coin(Position, 3, true);
                c.GridCollisionCheckDirections = new HashSet<Direction>() { Direction.UP, Direction.DOWN, Direction.LEFT, Direction.RIGHT };
                c.Velocity += new Vector2(random.Next(-2, 2), random.Next(-5, 0));;
            }
            Layer.Camera.Shake(2f, 0.5f);
            Destroy();
        }

        protected override void OnCircleCollisionStart(Entity otherCollider, CollisionResult collisionResult)
        {
            if (otherCollider is Carrot && IsMovingAtLeast(0.5f))
            {
                otherCollider.Destroy();
                Explode();
            }
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
