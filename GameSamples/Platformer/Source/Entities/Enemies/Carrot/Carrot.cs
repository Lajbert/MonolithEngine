using MonolithEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using ForestPlatformerExampl;

namespace ForestPlatformerExample
{
    class Carrot : AbstractEnemy
    {

        //private Direction CurrentFaceDirection;

        //private int direction = 1;

        private int health = 2;

        private Hero hero = null;

        private List<Vector2> line = new List<Vector2>();

        private bool seesHero = false;

        public bool OverlapsWithHero = false;

        public Carrot(AbstractScene scene, Vector2 position, Direction currentFaceDirection) : base(scene, position)
        {
            //SetSprite(SpriteUtil.CreateRectangle(16, Color.Orange));

            DrawPriority = 1;

            AddComponent(new CircleCollisionComponent(this, 12, new Vector2(3, -15)));

            AddComponent(new CarrotAIStateMachine(new CarrotPatrolState(this)));
            GetComponent<CarrotAIStateMachine>().AddState(new CarrotChaseState(this));
            GetComponent<CarrotAIStateMachine>().AddState(new CarrotIdleState(this));

            //RayEmitter = new Ray2DEmitter(this, 0, 360, 5, 100);

            AddComponent(new BoxTrigger(this, 300, 300, new Vector2(-150, -150), "vision"));

#if DEBUG
            //(GetComponent<ITrigger>() as AbstractTrigger).DEBUG_DISPLAY_TRIGGER = true;
            //DEBUG_SHOW_PIVOT = true;
#endif

            CurrentFaceDirection = currentFaceDirection;

            CollisionOffsetBottom = 1;
            CollisionOffsetLeft = 0.7f;
            CollisionOffsetRight = 0.7f;

            AnimationStateMachine Animations = new AnimationStateMachine();
            AddComponent(Animations);
            Animations.Offset = new Vector2(3, -33);

            SpriteSheetAnimation moveLeft = new SpriteSheetAnimation(this, Assets.GetTexture("CarrotMove"), 12);
            Animations.RegisterAnimation("MoveLeft", moveLeft, () => CurrentFaceDirection == Direction.WEST && Transform.VelocityX != 0);

            void setSpeed(int frame)
            {
                if (frame > 3 && frame < 8)
                {
                    CurrentSpeed = DefaultSpeed;
                }
                else
                {
                    CurrentSpeed = 0.001f;
                }
            }
            moveLeft.EveryFrameAction = setSpeed;

            SpriteSheetAnimation moveRight = moveLeft.CopyFlipped();
            Animations.RegisterAnimation("MoveRight", moveRight, () => CurrentFaceDirection == Direction.EAST && Transform.VelocityX != 0);

            Animations.AddFrameTransition("MoveLeft", "MoveRight");

            SpriteSheetAnimation hurtLeft = new SpriteSheetAnimation(this, Assets.GetTexture("CarrotHurt"), 24)
            {
                Looping = false
            };
            hurtLeft.StoppedCallback = () =>
            {
                CurrentSpeed = DefaultSpeed;
            };
            Animations.RegisterAnimation("HurtLeft", hurtLeft, () => false);

            SpriteSheetAnimation hurtRight = hurtLeft.CopyFlipped();
            Animations.RegisterAnimation("HurtRight", hurtRight, () => false);

            SpriteSheetAnimation deathLeft = new SpriteSheetAnimation(this, Assets.GetTexture("CarrotDeath"), 24)
            {
                Looping = false
            };
            Animations.RegisterAnimation("DeathLeft", deathLeft, () => false);

            SpriteSheetAnimation deathRight = deathLeft.CopyFlipped();
            Animations.RegisterAnimation("DeathRight", deathRight, () => false);

            SpriteSheetAnimation idleLeft = new SpriteSheetAnimation(this, Assets.GetTexture("CarrotIdle"), 24);
            Animations.RegisterAnimation("IdleLeft", idleLeft, () => Transform.VelocityX == 0 && CurrentFaceDirection == Direction.WEST);

            SpriteSheetAnimation idleRight = idleLeft.CopyFlipped();
            Animations.RegisterAnimation("IdleRight", idleRight, () => Transform.VelocityX == 0 && CurrentFaceDirection == Direction.EAST);

            SpriteSheetAnimation fallLeft = new SpriteSheetAnimation(this, Assets.GetTexture("CarrotMove"), 2);
            fallLeft.Looping = true;
            fallLeft.StartFrame = 5;
            fallLeft.EndFrame = 6;
            Animations.RegisterAnimation("FallLeft", fallLeft, () => Transform.VelocityY > 0 && !IsOnGround && CurrentFaceDirection == Direction.WEST, 2);

            SpriteSheetAnimation fallRight = fallLeft.CopyFlipped();
            Animations.RegisterAnimation("FallRight", fallRight, () => Transform.VelocityY > 0 && !IsOnGround && CurrentFaceDirection == Direction.EAST, 2);

            SetDestroyAnimation(deathRight, Direction.EAST);
            SetDestroyAnimation(deathLeft, Direction.WEST);

            Active = true;
            Visible = true;

            BlocksRay = true;

            /*DebugFunction = () =>
            {
                return Animations.GetCurrentAnimationState();
            };*/

            //SetSprite(SpriteUtil.CreateRectangle(Config.GRID, Color.Red));
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (BeingDestroyed || Destroyed)
            {
                return;
            }

            if (hero != null)
            {
                line.Clear();
                Bresenham.GetLine(Transform.Position + new Vector2(0, -15), hero.Transform.Position + new Vector2(0, -10), line);
                seesHero = Bresenham.CanLinePass(Transform.Position + new Vector2(0, -15), hero.Transform.Position + new Vector2(0, -10), (x, y) => {
                    return Scene.GridCollisionChecker.HasBlockingColliderAt(new Vector2(x / Config.GRID, y / Config.GRID));
                });

                if (seesHero)
                {
                    //seesHero &= (CurrentFaceDirection == Direction.EAST && hero.Transform.X > Transform.X || CurrentFaceDirection == Direction.WEST && hero.Transform.X < Transform.X);
                    if (hero.Transform.X < Transform.X)
                    {
                        CurrentFaceDirection = Direction.WEST;
                    }
                    else
                    {
                        CurrentFaceDirection = Direction.EAST;
                    }
                }
                
            } 
            else
            {
                seesHero = false;
            }

            if (seesHero)
            {
                if (!OverlapsWithHero && Math.Abs(hero.Transform.X - Transform.X) < 10)
                {
                    GetComponent<CarrotAIStateMachine>().ChangeState<CarrotIdleState>();
                } 
                else
                {
                    GetComponent<CarrotAIStateMachine>().ChangeState<CarrotChaseState>();
                }
            } 
            else
            {
                GetComponent<CarrotAIStateMachine>().ChangeState<CarrotPatrolState>();
            }

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
#if DEBUG
            /*if (seesHero)
            {
                foreach (Vector2 point in line)
                {
                    spriteBatch.Draw(Assets.CreateRectangle(1, Color.Red), point, Color.White);
                }
            }
            else
            {
                foreach (Vector2 point in line)
                {
                    spriteBatch.Draw(Assets.CreateRectangle(1, Color.Blue), point, Color.White);
                }
            }*/
            //line.Clear();
#endif
        }

        public override void Hit(Direction impactDirection)
        {
            if (health == 0)
            {
                CurrentSpeed = 0;
                RemoveCollisions();
                Die();
                return;
            }

            health--;
            CurrentSpeed = 0;
            PlayHurtAnimation();
            AudioEngine.Play("CarrotJumpHurtSound");
            if (impactDirection == Direction.NORTH)
            {
                CurrentSpeed = 0;
                Timer.TriggerAfter(300, () => CurrentSpeed = DefaultSpeed);
                return;
            }
            base.Hit(impactDirection);
        }

        public override void Die()
        {
            AudioEngine.Play("CarrotExplodeSound");
            base.Die();
        }

        private void PlayHurtAnimation()
        {
            if (CurrentFaceDirection == Direction.WEST)
            {
                GetComponent<AnimationStateMachine>().PlayAnimation("HurtLeft");
            }
            else
            {
                GetComponent<AnimationStateMachine>().PlayAnimation("HurtRight");
            }
        }

        public override void OnEnterTrigger(string triggerTag, IGameObject otherEntity)
        {
            if (otherEntity is Hero)
            {
                hero = otherEntity as Hero;
            }
            
        }

        public override void OnLeaveTrigger(string triggerTag, IGameObject otherEntity)
        {
            line.Clear();
            if (otherEntity is Hero)
            {
                hero = null;
            }
        }
    }
}
