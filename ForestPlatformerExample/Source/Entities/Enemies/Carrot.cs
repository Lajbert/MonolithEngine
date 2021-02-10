using ForestPlatformerExample.Source.Entities.Enemies;
using ForestPlatformerExample.Source.Entities.Interfaces;
using GameEngine2D;
using GameEngine2D.Engine.Source.Entities;
using GameEngine2D.Engine.Source.Entities.Animations;
using GameEngine2D.Engine.Source.Physics.Collision;
using GameEngine2D.Engine.Source.Physics.Raycast;
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
    class Carrot : AbstractEnemy
    {

        private float speed = 0.01f;

        public float CurrentSpeed = 0.01f;

        //private Direction CurrentFaceDirection;

        private int direction = 1;

        private int health = 2;

        public Carrot(Vector2 position, Direction CurrentFaceDirection) : base(position)
        {
            //SetSprite(SpriteUtil.CreateRectangle(16, Color.Orange));

            CollisionComponent = new CircleCollisionComponent(this, 12, new Vector2(3, -15));

            //DEBUG_SHOW_PIVOT = true;
            //DEBUG_SHOW_CIRCLE_COLLIDER = true;

            //RayEmitter = new Ray2DEmitter(this, 0, 360, 5, 100);

            Pivot = new Vector2(Config.GRID / 4, Config.GRID / 4);

            this.CurrentFaceDirection = CurrentFaceDirection;

            if (CurrentFaceDirection == Direction.LEFT)
            {
                SetLeftCollisionChecks();
            } 
            else if (CurrentFaceDirection == Direction.RIGHT)
            {
                SetRightCollisionChecks();
            }

            CollisionOffsetBottom = 1;

            Animations = new AnimationStateMachine();
            Animations.Offset = new Vector2(3, -33);
            SpriteSheetAnimation moveLeft = new SpriteSheetAnimation(this, "ForestAssets/Characters/Carrot/carrot@move-sheet", 12);
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

            SpriteSheetAnimation hurtLeft = new SpriteSheetAnimation(this, "ForestAssets/Characters/Carrot/carrot@hurt-sheet", 24);
            hurtLeft.Looping = false;
            Animations.RegisterAnimation("HurtLeft", hurtLeft, () => false);

            SpriteSheetAnimation hurtRight = hurtLeft.CopyFlipped();
            Animations.RegisterAnimation("HurtRight", hurtRight, () => false);

            SpriteSheetAnimation deathLeft = new SpriteSheetAnimation(this, "ForestAssets/Characters/Carrot/carrot@death-sheet", 24);
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
                return GridCollisionChecker.HasBlockingColliderAt(GridCoordinates, Direction.LEFT) || !GridCollisionChecker.HasBlockingColliderAt(GridCoordinates, Direction.BOTTOMLEFT);
            }
            else if (CurrentFaceDirection == Direction.RIGHT)
            {
                return GridCollisionChecker.HasBlockingColliderAt(GridCoordinates, Direction.RIGHT) || !GridCollisionChecker.HasBlockingColliderAt(GridCoordinates, Direction.BOTTOMRIGHT);
            }
            throw new Exception("Wrong CurrentFaceDirection for carrot!");
        }

        public override void Hit(Direction impactDirection)
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
