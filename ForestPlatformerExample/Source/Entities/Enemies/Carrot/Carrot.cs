using ForestPlatformerExample.Source.Entities.Enemies;
using ForestPlatformerExample.Source.Entities.Enemies.CarrotAI;
using ForestPlatformerExample.Source.Entities.Interfaces;
using ForestPlatformerExample.Source.PlayerCharacter;
using MonolithEngine;
using MonolithEngine.Engine.AI;
using MonolithEngine.Engine.Source.Entities;
using MonolithEngine.Engine.Source.Entities.Abstract;
using MonolithEngine.Engine.Source.Entities.Animations;
using MonolithEngine.Engine.Source.Physics.Bresenham;
using MonolithEngine.Engine.Source.Physics.Collision;
using MonolithEngine.Engine.Source.Physics.Raycast;
using MonolithEngine.Engine.Source.Physics.Trigger;
using MonolithEngine.Engine.Source.Util;
using MonolithEngine.Entities;
using MonolithEngine.Global;
using MonolithEngine.Source.Entities;
using MonolithEngine.Source.Util;
using MonolithEngine.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using MonolithEngine.Engine.Source.Scene;
using MonolithEngine.Engine.Source.Asset;

namespace ForestPlatformerExample.Source.Enemies
{
    class Carrot : AbstractEnemy
    {

        public float DefaultSpeed= 0.01f;

        public float CurrentSpeed = 0.01f;

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

            AddComponent(new CircleCollisionComponent(this, 12, new Vector2(3, -15)));

            AddComponent(new CarrotAIStateMachine(new CarrotPatrolState(this)));
            GetComponent<CarrotAIStateMachine>().AddState(new CarrotChaseState(this));
            GetComponent<CarrotAIStateMachine>().AddState(new CarrotIdleState(this));

            //DEBUG_SHOW_PIVOT = true;

            //RayEmitter = new Ray2DEmitter(this, 0, 360, 5, 100);

            Pivot = new Vector2(Config.GRID / 4, Config.GRID / 4);

            AddComponent(new BoxTrigger(300, 300, new Vector2(-150, -150), "vision", showTrigger:true));

            CurrentFaceDirection = currentFaceDirection;

            CollisionOffsetBottom = 1;
            CollisionOffsetLeft = 0.7f;
            CollisionOffsetRight = 0.7f;

            AnimationStateMachine Animations = new AnimationStateMachine();
            AddComponent(Animations);
            Animations.Offset = new Vector2(3, -33);

            SpriteSheetAnimation moveLeft = new SpriteSheetAnimation(this, Assets.GetTexture("CarrotMove"), 12);
            Animations.RegisterAnimation("MoveLeft", moveLeft, () => CurrentFaceDirection == Direction.WEST && Velocity.X != 0);

            void setSpeed(int frame)
            {
                if (frame > 3 && frame < 8)
                {
                    CurrentSpeed = DefaultSpeed;
                }
                else
                {
                    CurrentSpeed = 0;
                }
            }
            moveLeft.EveryFrameAction = setSpeed;

            SpriteSheetAnimation moveRight = moveLeft.CopyFlipped();
            Animations.RegisterAnimation("MoveRight", moveRight, () => CurrentFaceDirection == Direction.EAST && Velocity.X != 0);

            Animations.AddFrameTransition("MoveLeft", "MoveRight");

            SpriteSheetAnimation hurtLeft = new SpriteSheetAnimation(this, Assets.GetTexture("CarrotHurt"), 24)
            {
                Looping = false
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

            SpriteSheetAnimation idleLeft = new SpriteSheetAnimation(this, Assets.GetTexture("CarrotIdle"), 12);
            Animations.RegisterAnimation("IdleLeft", idleLeft, () => Velocity.X == 0 && CurrentFaceDirection == Direction.WEST);

            SpriteSheetAnimation idleRight = idleLeft.CopyFlipped();
            Animations.RegisterAnimation("IdleRight", idleRight, () => Velocity.X == 0 && CurrentFaceDirection == Direction.EAST);

            SetDestroyAnimation(deathRight, Direction.EAST);
            SetDestroyAnimation(deathLeft, Direction.WEST);

            Active = true;
            Visible = true;

            BlocksRay = true;

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
                    return Scene.GridCollisionChecker.HasBlockingColliderAt(new Vector2(x / Config.GRID, y / Config.GRID), Direction.CENTER);
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

            if (seesHero)
            {
                foreach (Vector2 point in line)
                {
                    spriteBatch.Draw(TextureUtil.CreateRectangle(1, Color.Red), point, Color.White);
                }
            }
            else
            {
                foreach (Vector2 point in line)
                {
                    spriteBatch.Draw(TextureUtil.CreateRectangle(1, Color.Blue), point, Color.White);
                }
            }
            //line.Clear();
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
                Timer.TriggerAfter(300, () => CurrentSpeed = DefaultSpeed);
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
            
            base.OnEnterTrigger(triggerTag, otherEntity);
        }

        public override void OnLeaveTrigger(string triggerTag, IGameObject otherEntity)
        {
            line.Clear();
            if (otherEntity is Hero)
            {
                hero = null;
            }
            base.OnLeaveTrigger(triggerTag, otherEntity);
        }
    }
}
