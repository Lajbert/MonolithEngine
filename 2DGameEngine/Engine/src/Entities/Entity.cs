using System;
using System.Collections.Generic;
using System.Text;
using GameEngine2D.Entities.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using GameEngine2D.Global;
using GameEngine2D.Util;
using GameEngine2D.src;
using GameEngine2D.src.Util;
using GameEngine2D.src.Layer;
using GameEngine2D.src.Entities.Animation;
using GameEngine2D.Engine.src.Entities.Animations;

namespace GameEngine2D.Entities
{
    class Entity : GameObject, Interfaces.IDrawable, IUpdatable, IHasParent, IHasChildren, ICollider
    {

        protected Vector2 startPosition;
        protected Vector2 currentPosition;
        protected Texture2D sprite;
        protected SpriteBatch spriteBatch;
        private HashSet<Entity> children;
        private HashSet<IUpdatable> updatables;
        private HashSet<Interfaces.IDrawable> drawables;
        private Entity parent;
        private bool hasCollision;
        protected GraphicsLayer layer;

        /*protected AbstractAnimation currentAnimation = null;
        protected AbstractAnimation idleAnimationLeft = null;
        protected AbstractAnimation idleAnimationRight = null;
        protected AbstractAnimation idleAnimationUp = null;
        protected AbstractAnimation idleAnimationDown = null;
        protected AbstractAnimation moveLeftAnimation = null;
        protected AbstractAnimation moveRightAnimation = null;
        protected AbstractAnimation moveUpAnimation = null;
        protected AbstractAnimation moveDownAnimation = null;
        protected AbstractAnimation jumpLeftAnimation = null;
        protected AbstractAnimation jumpRightAnimation = null;*/

#if GRAPHICS_DEBUG
        private Texture2D pivot;
        protected SpriteFont font;
#endif

        //grid coordinates
        //private float cx = 0f;
        //private float cy = 0f;
        protected Vector2 gridCoord;

        //between 0 and 1: where the object is inside the grid cell
        //private float xr = 0.5f;
        //private float yr = 1.0f;
        protected Vector2 inCellLocation;

        //private float dx = 0;
        //private float dy = 0;
        protected Vector2 direction = Vector2.Zero;

        protected AnimationStateMachine animationStates;

        public Entity(GraphicsLayer layer, Entity parent, GraphicsDevice graphicsDevice, Vector2 startPosition, SpriteFont font = null)
        {
            this.layer = layer;
            if (graphicsDevice != null)
            {
                spriteBatch = new SpriteBatch(graphicsDevice);
            }
            gridCoord = CalculateGridCoord(startPosition);
            this.children = new HashSet<Entity>();
            this.updatables = new HashSet<IUpdatable>();
            this.drawables = new HashSet<Interfaces.IDrawable>();
            if (parent != null) {
                this.parent = parent;
                this.parent.AddChild(this);
                this.startPosition = this.currentPosition = startPosition + GetParent().GetPositionWithParent();
            } else
            {
                RootContainer.Instance.AddChild(this);
                this.startPosition = this.currentPosition = startPosition + layer.GetPosition();
            }
            
            this.hasCollision = true;
            //this.startPosition = this.currentPosition = startPosition;

#if GRAPHICS_DEBUG
            pivot = CreateCircle(graphicsDevice, Constants.PIVOT_DIAM);
            this.font = font;
#endif
            layer.AddObject(this);
        }

#if GRAPHICS_DEBUG
        Texture2D CreateCircle(GraphicsDevice graphicsDevice, int radius)
        {
            Texture2D texture = new Texture2D(graphicsDevice, radius, radius);
            Color[] colorData = new Color[radius * radius];

            float diam = radius / 2f;
            float diamsq = diam * diam;

            for (int x = 0; x < radius; x++)
            {
                for (int y = 0; y < radius; y++)
                {
                    int index = x * radius + y;
                    Vector2 pos = new Vector2(x - diam, y - diam);
                    if (pos.LengthSquared() <= diamsq)
                    {
                        colorData[index] = Color.Red;
                    }
                    else
                    {
                        colorData[index] = Color.Transparent;
                    }
                }
            }

            texture.SetData(colorData);
            return texture;
        }
#endif


        public virtual void PreDraw(GameTime gameTime)
        {

            foreach (Interfaces.IDrawable child in drawables)
            {
                child.PreDraw(gameTime);
            }
        }

        public virtual void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            //Rectangle sourceRectangle = new Rectangle(0, 0, sprite.Width, sprite.Height);
            //private Vector2 origin = new Vector2(-16f, 0f);
            Vector2 pos;
            if (GetParent() != null)
            {
                pos = startPosition + GetParent().GetPositionWithParent();
            } else
            {
                
                pos = currentPosition + layer.GetPosition();
            }

            //spriteBatch.Draw(sprite, pos, sourceRectangle, Color.White, 0f, origin, 1f, SpriteEffects.None, 0);
            //if (this is ControllableEntity)
            //    spriteBatch.Draw(sprite, pos + (new Vector2(-Constants.SPRITE_DRAW_OFFSET, -Constants.SPRITE_DRAW_OFFSET) * Constants.GRID), Color.White);
            //else

            /*if (currentAnimation != null)
            {
                currentAnimation.Play(pos);
            } else if (sprite != null)
            {
                spriteBatch.Draw(sprite, pos, Color.White);
            }*/

#if GRAPHICS_DEBUG
            spriteBatch.Draw(pivot, pos, Color.White);
#endif

#if GRAPHICS_DEBUG
            if (font != null)
            {
                if (GetParent() != null)
                {
                    spriteBatch.DrawString(font, CalculateGridCoord().X + "\n" + CalculateGridCoord().Y, startPosition + GetParent().GetPositionWithParent(), Color.White);
                } else
                {
                    spriteBatch.DrawString(font, CalculateGridCoord().X + "\n" + CalculateGridCoord().Y, currentPosition + layer.GetPosition(), Color.White);
                }
                
            }
#endif

            spriteBatch.End();

            foreach (Interfaces.IDrawable child in drawables)
            {
                child.Draw(gameTime);
            }
        }

        public bool IsIdle()
        {
            return MathUtil.SmallerEqualAbs(direction, new Vector2(0.5f, 0.5f));
        }

        public virtual void PostDraw(GameTime gameTime)
        {

            foreach (Interfaces.IDrawable child in drawables)
            {
                child.PostDraw(gameTime);
            }
        }

        public virtual void PreUpdate(GameTime gameTime)
        {
            foreach (IUpdatable child in updatables)
            {
                child.PreUpdate(gameTime);
            }
        }

        public virtual void Update(GameTime gameTime)
        {

            /*if (currentAnimation != null)
            {
                currentAnimation.Update(gameTime);
            }*/

            foreach (IUpdatable child in updatables)
            {
                child.Update(gameTime);
            }
        }

        public virtual void PostUpdate(GameTime gameTime)
        {
            foreach (IUpdatable child in updatables)
            {
                child.PostUpdate(gameTime);
            }
        }

        public HashSet<Entity> GetAllChildren()
        {
            return children;
        }

        public void AddChild(Entity gameObject)
        {
            children.Add(gameObject);
            if (gameObject is Interfaces.IDrawable)
            {
                drawables.Add(gameObject);
            }
            if (gameObject is IUpdatable)
            {
                updatables.Add((IUpdatable)gameObject);
            }
        }

        public void RemoveChild(Entity gameObject)
        {
            children.Remove(gameObject);
            if (gameObject is Interfaces.IDrawable)
            {
                drawables.Remove(gameObject);
            }
            if (gameObject is IUpdatable)
            {
                updatables.Remove((IUpdatable)gameObject);
            }
        }

        public Entity GetParent()
        {
            return parent;
        }

        public override void Destroy()
        {
            parent.RemoveChild(this);
            if (!children.Any())
            {
                foreach (Entity o in children) {
                    if (o != null) {
                        o.Destroy();
                    }
                }
            }
        }

        public bool HasCollision()
        {
            return this.hasCollision;
        }

        public void SetCollisions(bool detectCollision)
        {
            this.hasCollision = detectCollision;
        }

        public Vector2 GetPosition()
        {
            return this.currentPosition;
            /*if (GetParent() != null)
            {
                return position + GetParent().GetPosition();
            }
            else
            {
                return position + RootContainer.Instance.GetRootPosition();
            }*/
        }

                public void SetSprite(Texture2D texture)
        {
            this.sprite = texture;
        }

        public Vector2 GetPositionWithParent()
        {
            if (GetParent() != null)
            {
                return currentPosition + GetParent().GetPositionWithParent();
            }
            else
            {
                return currentPosition + layer.GetPosition();
            }
        }

        public Vector2 GetStartPosition()
        {
            return this.startPosition;
            /*if (GetParent() != null)
            {
                return position + GetParent().GetPosition();
            }
            else
            {
                return position + RootContainer.Instance.GetRootPosition();
            }*/
        }

        public Vector2 GetCenter()
        {
            return currentPosition;
        }

        protected Vector2 CalculateGridCoord()
        {
            return CalculateGridCoord(currentPosition);
        }

        protected Vector2 CalculateGridCoord(Vector2 position)
        {
            return new Vector2((int)Math.Floor(position.X / Constants.GRID), (int)Math.Floor(position.Y / Constants.GRID));
        }

        public Vector2 GetGridCoord()
        {
            return gridCoord;
        }

        protected HashSet<IUpdatable> GetUpdatables()
        {
            return updatables;
        }

        protected HashSet<Interfaces.IDrawable> GetDrawables()
        {
            return drawables;
        }

        public void AddParent(Entity newParent)
        {
            if (parent != null)
            {
                parent.RemoveChild(this);
                parent.AddChild(newParent);
                newParent.AddChild(this);
                parent = newParent;
            } else
            {
                RootContainer.Instance.RemoveChild(this);
                RootContainer.Instance.AddChild(newParent);
                newParent.AddChild(this);
                parent = newParent;
            }
        }

        /*public void SetIdleAnimationLeft(AbstractAnimation idleAnimationLeft)
        {
            this.idleAnimationLeft = idleAnimationLeft;
            this.currentAnimation = idleAnimationLeft;
        }

        public void SetIdleAnimationRight(AbstractAnimation idleAnimationRight)
        {
            this.idleAnimationRight = idleAnimationRight;
            this.currentAnimation = idleAnimationRight;
        }

        public void SetMoveRightAnimation(AbstractAnimation moveRightAnimation)
        {
            this.moveRightAnimation = moveRightAnimation;
        }

        public void SetMoveLeftAnimation(AbstractAnimation moveLeftAnimation)
        {
            this.moveLeftAnimation = moveLeftAnimation;
        }
        public void SetJumpLeftAnimation(AbstractAnimation jumpLeftAnimation)
        {
            this.jumpLeftAnimation = jumpLeftAnimation;
        }

        public void SetJumpRightAnimation(AbstractAnimation jumpRightAnimation)
        {
            this.jumpRightAnimation = jumpRightAnimation;
        }*/

        public void SetAnimationStateMachime(AnimationStateMachine animationStates)
        {
            this.animationStates = animationStates;
        }
    }
}
