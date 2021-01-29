using GameEngine2D;
using GameEngine2D.Engine.Source.Entities;
using GameEngine2D.Engine.Source.Entities.Animations;
using GameEngine2D.Engine.Source.Util;
using GameEngine2D.Entities;
using GameEngine2D.Global;
using GameEngine2D.Source.Entities;
using GameEngine2D.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForestPlatformerExample.Source.Enemies
{
    class Carrot : PhysicalEntity
    {

        private float speed = 0.01f;

        public float CurrentSpeed = 0.01f;

        //private Direction CurrentFaceDirection;

        private int direction = 1;

        public Carrot(Vector2 position, Direction CurrentFaceDirection) : base(LayerManager.Instance.EntityLayer, null, position)
        {
            //SetSprite(SpriteUtil.CreateRectangle(16, Color.Orange));

            DEBUG_SHOW_PIVOT = true;

            CollisionPriority = 1;

            CheckForCollisions = true;

            Pivot = new Vector2(Config.GRID / 4, Config.GRID / 4);

            AddTag("MovingEnemy");

            this.CurrentFaceDirection = CurrentFaceDirection;

            if (CurrentFaceDirection == Direction.LEFT)
            {
                SetLeftCollisionChecks();
            } 
            else if (CurrentFaceDirection == Direction.RIGHT)
            {
                SetRightCollisionChecks();
            }

            Animations = new AnimationStateMachine();
            Animations.Offset = new Vector2(3, -20);
            Texture2D spriteSheet = SpriteUtil.LoadTexture("Green_Greens_Forest_Pixel_Art_Platformer_Pack/Character-Animations/Enemy-Carrot/carrot@move-sheet");
            SpriteSheetAnimation moveLeft = new SpriteSheetAnimation(this, spriteSheet, 1, 10, 10, 64, 64, 12);
            Animations.RegisterAnimation("MoveLeft", moveLeft, () => this.CurrentFaceDirection == Direction.LEFT);

            Action<int> setSpeed = frame =>
            {
                if (frame > 3 && frame < 8)
                {
                    CurrentSpeed = speed;
                }
                else
                {
                    CurrentSpeed = 0;
                }
            };
            moveLeft.EveryFrameAction = setSpeed;

            SpriteSheetAnimation moveRight = moveLeft.CopyFlipped();
            Animations.RegisterAnimation("MoveRight", moveRight, () => this.CurrentFaceDirection == Direction.RIGHT);

            Animations.AddFrameTransition("MoveLeft", "MoveRight");

            spriteSheet = SpriteUtil.LoadTexture("Green_Greens_Forest_Pixel_Art_Platformer_Pack/Character-Animations/Enemy-Carrot/carrot@hurt-sheet");
            SpriteSheetAnimation hurtLeft = new SpriteSheetAnimation(this, spriteSheet, 1, 8, 8, 64, 64, 24);
            hurtLeft.Looping = false;
            Animations.RegisterAnimation("HurtLeft", hurtLeft, () => false);

            SpriteSheetAnimation hurtRight = hurtLeft.CopyFlipped();
            Animations.RegisterAnimation("HurtRight", hurtRight, () => false);

            Active = true;
            Visible = true;

            BlocksRay = true;
        }

        public void Hit(Direction direction)
        {
            if (CurrentFaceDirection == Direction.LEFT)
            {
                Animations.PlayAnimation("HurtLeft");
            } else
            {
                Animations.PlayAnimation("HurtRight");
            }
            Velocity = Vector2.Zero;
            Vector2 attackForce = new Vector2(5, -5);
            if (direction == Direction.LEFT)
            {
                attackForce.X *= -1;
                Velocity += attackForce;
            }
            else if (direction == Direction.RIGHT)
            {
                Velocity += attackForce;
            }
        }

        private void SetLeftCollisionChecks()
        {
            CollisionCheckDirections.Clear();
            CollisionCheckDirections.Add(Direction.LEFT);
            CollisionCheckDirections.Add(Direction.BOTTOMLEFT);
        }

        private void SetRightCollisionChecks()
        {
            CollisionCheckDirections.Clear();
            CollisionCheckDirections.Add(Direction.RIGHT);
            CollisionCheckDirections.Add(Direction.BOTTOMRIGHT);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (WillCollideOrFall())
            {
                if (CurrentFaceDirection == Direction.LEFT)
                {
                    SetLeftCollisionChecks();
                    CurrentFaceDirection = Direction.RIGHT;
                } 
                else if (CurrentFaceDirection == Direction.RIGHT)
                {
                    SetRightCollisionChecks();
                    CurrentFaceDirection = Direction.LEFT;
                }
                direction *= -1;
            }

            //Logger.Log("Speed * direction * gameTime.ElapsedGameTime.Milliseconds: " + (Speed * direction * gameTime.ElapsedGameTime.Milliseconds));

            //X += Speed * direction * gameTime.ElapsedGameTime.Milliseconds;
            Velocity.X += CurrentSpeed * direction * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
        }

        private bool WillCollideOrFall()
        {
            if (CurrentFaceDirection == Direction.LEFT)
            {
                return CollisionChecker.HasBlockingColliderAt(GridUtil.GetLeftGrid(GridCoordinates)) || !CollisionChecker.HasBlockingColliderAt(GridUtil.GetLeftBelowGrid(GridCoordinates));
            }
            else if (CurrentFaceDirection == Direction.RIGHT)
            {
                return CollisionChecker.HasBlockingColliderAt(GridUtil.GetRightGrid(GridCoordinates)) || !CollisionChecker.HasBlockingColliderAt(GridUtil.GetRightBelowGrid(GridCoordinates));
            }
            throw new Exception("Wrong CurrentFaceDirection for carrot!");
        }
    }
}
