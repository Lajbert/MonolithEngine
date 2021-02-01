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

            this.bumps = currentBump = bumps;

            random = new Random();

            CircleCollider = new CircleCollider(this, 10, new Vector2(4, -8));
            EnableCircleCollisions = false;

            CollisionOffsetBottom = 0.6f;
            //CollisionOffsetLeft = 0.9f;
            //CollisionOffsetRight = 0.9f;

            GravityValue /= 2;
            Friction = 0.6f;

            //BlocksMovement = true;
            //ColliderOnGrid = true;

            Active = true;

            //AddBlockedDirection(Direction.UP);
            //AddBlockedDirection(Direction.LEFT);
            //AddBlockedDirection(Direction.RIGHT);

            //DEBUG_SHOW_PIVOT = true;
            //DEBUG_SHOW_CIRCLE_COLLIDER = true;

            Animations = new AnimationStateMachine();
            Animations.Offset = new Vector2(4, -15);

            Texture2D spriteSheet = SpriteUtil.LoadTexture("Green_Greens_Forest_Pixel_Art_Platformer_Pack/Items-and-Objects/Sprite-Sheets/box-idle");
            SpriteSheetAnimation boxIdle = new SpriteSheetAnimation(this, spriteSheet, 2, 7, 13, 32, 32, 24);
            Animations.RegisterAnimation("BoxIdle", boxIdle);

            spriteSheet = SpriteUtil.LoadTexture("Green_Greens_Forest_Pixel_Art_Platformer_Pack/Items-and-Objects/Sprite-Sheets/box-hit");
            SpriteSheetAnimation boxHit = new SpriteSheetAnimation(this, spriteSheet, 1, 5, 5, 32, 32, 24);
            boxHit.Looping = false;
            Animations.RegisterAnimation("BoxHit", boxHit);

            spriteSheet = SpriteUtil.LoadTexture("Green_Greens_Forest_Pixel_Art_Platformer_Pack/Items-and-Objects/Sprite-Sheets/box-destroy");
            SpriteSheetAnimation boxDestroy = new SpriteSheetAnimation(this, spriteSheet, 1, 8, 8, 32, 32, 24);
            boxDestroy.StartedCallback += () => Pop();
            boxDestroy.Looping = false;
            SetDestroyAnimation(boxDestroy);

            //SetSprite(SpriteUtil.CreateRectangle(Config.GRID, Color.Brown));
        }

        public override void Update(GameTime gameTime)
        {
            /*if (!OnGround())
            {
                Friction = 0.8f;
            } else
            {
                Friction = 0.3f;
            }*/
            base.Update(gameTime);
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

        public void Throw(Entity entity)
        {
            RemoveParent();
            EnablePhysics();
            FallSpeed = 0;
            Velocity = Vector2.Zero;
            if (entity.CurrentFaceDirection == Direction.LEFT)
            {
                Velocity += new Vector2(-5, -0.5f);
            } else
            {
                Velocity += new Vector2(5, -0.5f);
            }
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
            BlocksMovement = false;
            EnableCircleCollisions = false;
            GridCollisionCheckDirections = new HashSet<Direction>();
            ColliderOnGrid = false;
            HasGravity = false;
        }

        private void Pop()
        {
            int numOfCoins = random.Next(3, 6);
            for (int i = 0; i < numOfCoins; i++)
            {
                Coin c = new Coin(Position, 3);
                c.CircleCollider = null;
                c.EnableCircleCollisions = false;
                c.ColliderOnGrid = true;
                c.GridCollisionCheckDirections = new HashSet<Direction>() { Direction.UP, Direction.DOWN, Direction.LEFT, Direction.RIGHT };
                c.Velocity += new Vector2(random.Next(-5, 5), random.Next(-10, 0));
                c.HasGravity = true;
                Timer.TriggerAfter(1000, () => c.SetCircleCollider());
            }
            Destroy();
        }

        protected override void OnCircleCollisionStart(Entity otherCollider, float intersection)
        {
            if (otherCollider is Carrot && Velocity != Vector2.Zero)
            {
                otherCollider.Destroy();
                Explode();
            }
        }

        protected override void OnGridCollision(Entity otherCollider, Direction direction)
        {
            

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
            UpdateGridPosition = false;
            GravityValue = 0;
            Destroy();
        }
    }
}
