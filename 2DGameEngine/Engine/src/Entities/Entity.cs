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
using GameEngine2D.GameExamples2D.SideScroller.src;
using GameEngine2D.Engine.src.Entities.Interfaces;
using GameEngine2D.Engine.src.Physics.Raycast;
using GameEngine2D.Engine.src.Layer;

namespace GameEngine2D.Entities
{
    class Entity : GameObject, Interfaces.IDrawable, IUpdatable, IHasParent, IHasChildren, ICollider, IRayBlocker
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
        protected AbstractLayer layer;
        protected List<(Vector2 start, Vector2 end)> rayBlockerLines;
        private bool blocksRay = true;

#if GRAPHICS_DEBUG
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

        protected Ray2DEmitter rayEmitter;

        protected static GraphicsDeviceManager graphicsDeviceManager;

        public Entity(AbstractLayer layer, Entity parent, Vector2 startPosition, SpriteFont font = null)
        {
            this.layer = layer;
            spriteBatch = new SpriteBatch(graphicsDeviceManager.GraphicsDevice);
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
            
            hasCollision = true;
            rayBlockerLines = new List<(Vector2 start, Vector2 end)>();
            //this.startPosition = this.currentPosition = startPosition;

#if GRAPHICS_DEBUG
            this.font = font;
#endif
            layer.AddObject(this);
        }

        public virtual void SetRayBlockers()
        {
            rayBlockerLines.Add((Vector2.Zero, new Vector2(0, Config.GRID)));
            rayBlockerLines.Add((Vector2.Zero, new Vector2(Config.GRID, 0)));
            rayBlockerLines.Add((new Vector2(Config.GRID, 0), new Vector2(0, Config.GRID)));
            rayBlockerLines.Add((new Vector2(0, Config.GRID), new Vector2(Config.GRID, Config.GRID)));
        }

        public virtual void PreDraw(GameTime gameTime)
        {

            foreach (Interfaces.IDrawable child in drawables)
            {
                child.PreDraw(gameTime);
            }
        }

        public virtual void Draw(GameTime gameTime)
        {
            Vector2 position;
            if (GetParent() != null)
            {
                position = startPosition + GetParent().GetPositionWithParent();
            } else
            {
                
                position = currentPosition + layer.GetPosition();
            }

            if (animationStates != null)
            {
                animationStates.Draw(gameTime);
            }
            else
            {
                DrawSprite(position);
            }
#if GRAPHICS_DEBUG
            spriteBatch.Begin();
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
            spriteBatch.End();
#endif

            foreach (Interfaces.IDrawable child in drawables)
            {
                child.Draw(gameTime);
            }
        }


        protected virtual void DrawSprite(Vector2 position)
        {
            if (sprite != null)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(sprite, position, Color.White);
                spriteBatch.End();
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

            if (animationStates != null)
            {
                animationStates.Update(gameTime);
            }

            foreach (IUpdatable child in updatables)
            {
                child.Update(gameTime);
            }
        }

        public virtual void PostUpdate(GameTime gameTime)
        {

            if (rayEmitter != null)
            {
                rayEmitter.UpdateRays();
            }

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
            return new Vector2((int)Math.Floor(position.X / Config.GRID), (int)Math.Floor(position.Y / Config.GRID));
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

        public void SetAnimationStateMachime(AnimationStateMachine animationStates)
        {
            this.animationStates = animationStates;
        }

        public static void SetGraphicsDeviceManager(GraphicsDeviceManager graphics)
        {
            graphicsDeviceManager = graphics;
        }

        public void SetRayEmitter(Ray2DEmitter rayEmitter)
        {
            this.rayEmitter = rayEmitter;
        }

        public virtual List<(Vector2 start, Vector2 end)> GetRayBlockerLines()
        {
            return rayBlockerLines;
        }

        public bool BlocksRay()
        {
            return blocksRay;
        }

        public void SetPosition(Vector2 position)
        {
            this.currentPosition = position;
        }
    }
}
