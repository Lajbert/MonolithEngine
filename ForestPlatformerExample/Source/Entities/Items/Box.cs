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

        private int bounceX = 2;
        private int bounceY = 1;

        public Box(Vector2 position) : base(LayerManager.Instance.EntityLayer, null, position)
        {
            //ColliderOnGrid = true;

            random = new Random();

            CircleCollider = new CircleCollider(this, 10, new Vector2(4, -8));
            EnableCircleCollisions = false;

            CollisionOffsetBottom = 0.6f;
            CollisionOffsetLeft = 0.9f;
            CollisionOffsetRight = 0.9f;

            GravityValue /= 2;
            Friction = 0.4f;

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
            bounceX = 2;
            bounceY = 1;
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
            if (entity.CurrentFaceDirection == Direction.LEFT)
            {
                Velocity += new Vector2(-5, 0.1f);
            } else
            {
                Velocity += new Vector2(5, 0.1f);
            }
        }

        private void EnablePhysics()
        {
            GridCoordinates = CalculateGridCoord();
            //UpdateInCellCoord();
            InCellLocation = Vector2.Zero;
            GridCollisionCheckDirections = new HashSet<Direction>() { Direction.UP, Direction.DOWN, Direction.LEFT, Direction.RIGHT };
            //UpdateGridPosition = true;
            HasGravity = true;
            Active = true;
            //ColliderOnGrid = true;
            EnableCircleCollisions = true;
        }

        private void DisablePysics()
        {
            BlocksMovement = false;
            EnableCircleCollisions = false;
            GridCollisionCheckDirections = new HashSet<Direction>();
            ColliderOnGrid = false;
            HasGravity = false;
            //Active = false;
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

            if (otherCollider is Hero)
            {
                return;
            }
            //return;
            if (direction == Direction.DOWN && InCellLocation.Y >= CollisionOffsetBottom && bounceX > 0 && Velocity.Y >= 0)
            //if (direction == Direction.DOWN)
            {
                Logger.Log("BUMP!");
                bounceX--;
                Bump(new Vector2(0, Velocity.Y * -100));
            }

            //InCellLocation.X >= CollisionOffsetLeft && CollisionChecker.HasBlockingColliderAt(GridCoordinates, Direction.RIGHT))
            //if (HasGridCollision() && InCellLocation.X <= CollisionOffsetRight && CollisionChecker.HasBlockingColliderAt(GridCoordinates, Direction.LEFT))
            if (direction == Direction.LEFT && /*InCellLocation.X <= CollisionOffsetRight &&*/ bounceY > 0)
            {
                Bump(new Vector2(bounceY-- * 2, 0));
            }
            if (direction == Direction.RIGHT && /*InCellLocation.X >= CollisionOffsetLeft &&*/ bounceY > 0)
            {
                Bump(new Vector2(-bounceY-- * 2, 0));
            }
            //Vector2 bounce = -Velocity;

            //Bump(bounce);

            /*
            if (InCellLocation.Y >= CollisionOffsetBottom)
            {
                Logger.Log("Other collider: " + otherCollider);
                Logger.Log("Direction: " + direction);
                Explode();
            }*/

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
