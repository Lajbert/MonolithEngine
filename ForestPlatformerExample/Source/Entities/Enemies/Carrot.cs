using ForestPlatformerExample.Source.Entities.Interfaces;
using GameEngine2D;
using GameEngine2D.Engine.Source.Entities;
using GameEngine2D.Engine.Source.Entities.Animations;
using GameEngine2D.Engine.Source.Physics.Collision;
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
    class Carrot : PhysicalEntity, IAttackable
    {

        private float speed = 0.01f;

        public float CurrentSpeed = 0.01f;

        //private Direction CurrentFaceDirection;

        private int direction = 1;

        private int health = 2;

        public Carrot(Vector2 position, Direction CurrentFaceDirection) : base(LayerManager.Instance.EntityLayer, null, position)
        {
            //SetSprite(SpriteUtil.CreateRectangle(16, Color.Orange));

            GridCollisionPriority = 1;

            CircleCollider = new CircleCollider(this, 12, new Vector2(3, -3));

            //DEBUG_SHOW_PIVOT = true;
            //DEBUG_SHOW_CIRCLE_COLLIDER = true;

            ColliderOnGrid = true;

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

            spriteSheet = SpriteUtil.LoadTexture("Green_Greens_Forest_Pixel_Art_Platformer_Pack/Character-Animations/Enemy-Carrot/carrot@death-sheet");
            SpriteSheetAnimation deathLeft = new SpriteSheetAnimation(this, spriteSheet, 1, 10, 10, 64, 64, 24);
            deathLeft.Looping = false;
            Animations.RegisterAnimation("DeathLeft", deathLeft, () => false);

            SpriteSheetAnimation deathRight = deathLeft.CopyFlipped();
            Animations.RegisterAnimation("DeathRight", deathRight, () => false);

            SetDestroyAnimation(deathRight, Direction.RIGHT);
            SetDestroyAnimation(deathLeft, Direction.LEFT);

            Active = true;
            Visible = true;

            BlocksRay = true;

            //SetSprite(SpriteUtil.CreateRectangle(Config.GRID, Color.Red));
        }

        private void SetLeftCollisionChecks()
        {
            GridCollisionCheckDirections.Clear();
            GridCollisionCheckDirections.Add(Direction.LEFT);
            GridCollisionCheckDirections.Add(Direction.BOTTOMLEFT);
        }

        private void SetRightCollisionChecks()
        {
            GridCollisionCheckDirections.Clear();
            GridCollisionCheckDirections.Add(Direction.RIGHT);
            GridCollisionCheckDirections.Add(Direction.BOTTOMRIGHT);
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
                return CollisionChecker.HasBlockingColliderAt(GridCoordinates, Direction.LEFT) || !CollisionChecker.HasBlockingColliderAt(GridCoordinates, Direction.BOTTOMLEFT);
            }
            else if (CurrentFaceDirection == Direction.RIGHT)
            {
                return CollisionChecker.HasBlockingColliderAt(GridCoordinates, Direction.RIGHT) || !CollisionChecker.HasBlockingColliderAt(GridCoordinates, Direction.BOTTOMRIGHT);
            }
            throw new Exception("Wrong CurrentFaceDirection for carrot!");
        }

        public void Hit(Direction impactDirection)
        {
            if (health == 0)
            {
                CurrentSpeed = 0;
                RemoveCollisions();
                Destroy();
                return;
            }

            health--;
            PlayHurtAnimation();
            if (impactDirection == Direction.UP)
            {
                CurrentSpeed = 0;
                Timer.TriggerAfter(300, () => CurrentSpeed = speed);
                return;
            }

            
            Velocity = Vector2.Zero;
            Vector2 attackForce = new Vector2(5, -5);
            if (impactDirection == Direction.LEFT)
            {
                attackForce.X *= -1;
                Velocity += attackForce;
            }
            else if (impactDirection == Direction.RIGHT)
            {
                Velocity += attackForce;
            }
            FallSpeed = 0;
        }

        private void PlayHurtAnimation()
        {
            if (CurrentFaceDirection == Direction.LEFT)
            {
                Animations.PlayAnimation("HurtLeft");
            }
            else
            {
                Animations.PlayAnimation("HurtRight");
            }
        }
    }
}
