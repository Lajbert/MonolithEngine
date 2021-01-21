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
    class Carrot : Entity
    {

        private float speed = 0.1f;

        public float CurrentSpeed = 0.1f;

        private Direction faceDirection;

        private int direction = 1;

        public Carrot(Vector2 position, Direction faceDirection) : base(LayerManager.Instance.EntityLayer, null, position)
        {
            //SetSprite(SpriteUtil.CreateRectangle(16, Color.Orange));

            Pivot = new Vector2(Config.GRID / 4, Config.GRID / 4);

            this.faceDirection = faceDirection;

            if (faceDirection == Direction.LEFT)
            {
                SetLeftCollisionChecks();
            } 
            else if (faceDirection == Direction.RIGHT)
            {
                SetRightCollisionChecks();
            }

            Animations = new AnimationStateMachine();
            Animations.Offset = new Vector2(0, -20);
            Texture2D spriteSheet = SpriteUtil.LoadTexture("Green_Greens_Forest_Pixel_Art_Platformer_Pack/Character-Animations/Enemy-Carrot/carrot@move-sheet");
            SpriteSheetAnimation moveLeft = new SpriteSheetAnimation(this, spriteSheet, 1, 10, 10, 64, 64, 12);
            Animations.RegisterAnimation("MoveLeft", moveLeft, () => this.faceDirection == Direction.LEFT);

            SpriteSheetAnimation moveRight = new SpriteSheetAnimation(this, spriteSheet, 1, 10, 10, 64, 64, 12, SpriteEffects.FlipHorizontally);
            Animations.RegisterAnimation("MoveRight", moveRight, () => this.faceDirection == Direction.RIGHT);

            Action<int> setSpeed = frame =>
            {
                Logger.Log("CURENT FRAME" + frame);
                if (frame > 3 && frame < 8)
                {
                    CurrentSpeed = speed;
                }
                else
                {
                    CurrentSpeed = 0;
                }
            };

            moveLeft.FrameAction = setSpeed;
            moveRight.FrameAction = setSpeed;

            Animations.AddFrameTransition("MoveLeft", "MoveRight");

            Active = true;
            Visible = true;
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
                if (faceDirection == Direction.LEFT)
                {
                    SetLeftCollisionChecks();
                    faceDirection = Direction.RIGHT;
                } 
                else if (faceDirection == Direction.RIGHT)
                {
                    SetRightCollisionChecks();
                    faceDirection = Direction.LEFT;
                }
                direction *= -1;
            }

            //Logger.Log("Speed * direction * gameTime.ElapsedGameTime.Milliseconds: " + (Speed * direction * gameTime.ElapsedGameTime.Milliseconds));

            //X += Speed * direction * gameTime.ElapsedGameTime.Milliseconds;
            X += CurrentSpeed * direction * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
        }

        private bool WillCollideOrFall()
        {
            if (faceDirection == Direction.LEFT)
            {
                return CollisionChecker.HasBlockingColliderAt(GridUtil.GetLeftGrid(GridCoordinates)) || !CollisionChecker.HasBlockingColliderAt(GridUtil.GetLeftBelowGrid(GridCoordinates));
            }
            else if (faceDirection == Direction.RIGHT)
            {
                return CollisionChecker.HasBlockingColliderAt(GridUtil.GetRightGrid(GridCoordinates)) || !CollisionChecker.HasBlockingColliderAt(GridUtil.GetRightBelowGrid(GridCoordinates));
            }
            throw new Exception("Wrong facedirection for carrot!");
        }
    }
}
