using ForestPlatformerExample.Source.Entities.Enemies;
using ForestPlatformerExample.Source.Entities.Interfaces;
using ForestPlatformerExample.Source.PlayerCharacter;
using GameEngine2D;
using GameEngine2D.Engine.Source.Entities;
using GameEngine2D.Engine.Source.Entities.Abstract;
using GameEngine2D.Engine.Source.Entities.Animations;
using GameEngine2D.Engine.Source.Physics.Bresenham;
using GameEngine2D.Engine.Source.Physics.Collision;
using GameEngine2D.Engine.Source.Physics.Raycast;
using GameEngine2D.Engine.Source.Physics.Trigger;
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

        private Hero hero = null;

        public Carrot(Vector2 position, Direction CurrentFaceDirection) : base(position)
        {
            //SetSprite(SpriteUtil.CreateRectangle(16, Color.Orange));

            AddComponent(new CircleCollisionComponent(this, 12, new Vector2(3, -15)));

            //DEBUG_SHOW_PIVOT = true;

            //RayEmitter = new Ray2DEmitter(this, 0, 360, 5, 100);

            Pivot = new Vector2(Config.GRID / 4, Config.GRID / 4);

            AddTrigger(new BoxTrigger(300, 300, new Vector2(-150, -150), "vision", showTrigger:true));

            this.CurrentFaceDirection = CurrentFaceDirection;

            if (CurrentFaceDirection == Direction.WEST)
            {
                SetLeftCollisionChecks();
            } 
            else if (CurrentFaceDirection == Direction.EAST)
            {
                SetRightCollisionChecks();
            }

            CollisionOffsetBottom = 1;

            Animations = new AnimationStateMachine();
            Animations.Offset = new Vector2(3, -33);
            SpriteSheetAnimation moveLeft = new SpriteSheetAnimation(this, "ForestAssets/Characters/Carrot/carrot@move-sheet", 12);
            Animations.RegisterAnimation("MoveLeft", moveLeft, () => this.CurrentFaceDirection == Direction.WEST);

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
            Animations.RegisterAnimation("MoveRight", moveRight, () => this.CurrentFaceDirection == Direction.EAST);

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

            SetDestroyAnimation(deathRight, Direction.EAST);
            SetDestroyAnimation(deathLeft, Direction.WEST);

            Active = true;
            Visible = true;

            BlocksRay = true;

            //SetSprite(SpriteUtil.CreateRectangle(Config.GRID, Color.Red));
        }

        private void SetLeftCollisionChecks()
        {
            GridCollisionCheckDirections.Clear();
            GridCollisionCheckDirections.Add(Direction.WEST);
            GridCollisionCheckDirections.Add(Direction.SOUTHWEST);
        }

        private void SetRightCollisionChecks()
        {
            GridCollisionCheckDirections.Clear();
            GridCollisionCheckDirections.Add(Direction.EAST);
            GridCollisionCheckDirections.Add(Direction.SOUTHEAST);
        }

        private List<Vector2> line = new List<Vector2>();
        private bool canRayPass = false;
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (WillCollideOrFall())
            {
                if (CurrentFaceDirection == Direction.WEST)
                {
                    SetLeftCollisionChecks();
                    CurrentFaceDirection = Direction.EAST;
                } 
                else if (CurrentFaceDirection == Direction.EAST)
                {
                    SetRightCollisionChecks();
                    CurrentFaceDirection = Direction.WEST;
                }
                direction *= -1;
            }

            if (hero != null)
            {
                line.Clear();
                Bresenham.GetLine(Transform.Position + new Vector2(0, -15), hero.Transform.Position + new Vector2(0, -10), line);
                canRayPass = Bresenham.CanLinePass(Transform.Position + new Vector2(0, -15), hero.Transform.Position + new Vector2(0, -10), (x, y) => {
                    return GridCollisionChecker.Instance.HasBlockingColliderAt(new Vector2(x / Config.GRID, y / Config.GRID), Direction.CENTER);
                });
            }

            //Logger.Log("Speed * direction * gameTime.ElapsedGameTime.Milliseconds: " + (Speed * direction * gameTime.ElapsedGameTime.Milliseconds));

            //X += Speed * direction * gameTime.ElapsedGameTime.Milliseconds;
            Velocity.X += CurrentSpeed * direction * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.Draw(spriteBatch, gameTime);

            if (canRayPass)
            {
                foreach (Vector2 point in line)
                {
                    spriteBatch.Draw(SpriteUtil.CreateRectangle(1, Color.Red), point, Color.White);
                }
            }
            else
            {
                foreach (Vector2 point in line)
                {
                    spriteBatch.Draw(SpriteUtil.CreateRectangle(1, Color.Blue), point, Color.White);
                }
            }
            line.Clear();
        }

        private bool WillCollideOrFall()
        {
            if (CurrentFaceDirection == Direction.WEST)
            {
                return GridCollisionChecker.Instance.HasBlockingColliderAt(Transform.GridCoordinates, Direction.WEST) || !GridCollisionChecker.Instance.HasBlockingColliderAt(Transform.GridCoordinates, Direction.SOUTHWEST);
            }
            else if (CurrentFaceDirection == Direction.EAST)
            {
                return GridCollisionChecker.Instance.HasBlockingColliderAt(Transform.GridCoordinates, Direction.EAST) || !GridCollisionChecker.Instance.HasBlockingColliderAt(Transform.GridCoordinates, Direction.SOUTHEAST);
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
            if (impactDirection == Direction.NORTH)
            {
                CurrentSpeed = 0;
                Timer.TriggerAfter(300, () => CurrentSpeed = speed);
                return;
            }

            
            Velocity = Vector2.Zero;
            Vector2 attackForce = new Vector2(5, -5);
            if (impactDirection == Direction.WEST)
            {
                attackForce.X *= -1;
                Velocity += attackForce;
            }
            else if (impactDirection == Direction.EAST)
            {
                Velocity += attackForce;
            }
            FallSpeed = 0;
        }

        private void PlayHurtAnimation()
        {
            if (CurrentFaceDirection == Direction.WEST)
            {
                Animations.PlayAnimation("HurtLeft");
            }
            else
            {
                Animations.PlayAnimation("HurtRight");
            }
        }

        public override void OnEnterTrigger(string triggerTag, IGameObject otherEntity)
        {
            if (otherEntity is Hero)
            {
                hero = otherEntity as Hero;
            }
            
            base.OnEnterTrigger(triggerTag, otherEntity);
        }

        public override void OnLeaveTrigger(string triggerTag, IGameObject otherEntity)
        {
            if (otherEntity is Hero)
            {
                hero = null;
            }
            base.OnLeaveTrigger(triggerTag, otherEntity);
        }
    }
}
