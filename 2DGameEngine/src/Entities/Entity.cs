using System;
using System.Collections.Generic;
using System.Text;
using _2DGameEngine.Entities.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using _2DGameEngine.Global;
using _2DGameEngine.Util;
using _2DGameEngine.src;
using _2DGameEngine.src.Util;

namespace _2DGameEngine.Entities
{
    class Entity : GameObject, Drawable, HasParent, HasChildren, Collider
    {

        protected Vector2 startPosition;
        protected Vector2 currentPosition;
        protected Texture2D sprite;
        protected SpriteBatch spriteBatch;
        private HashSet<Entity> children;
        private HashSet<Updatable> updatables;
        private HashSet<Drawable> drawables;
        private Entity parent;
        private bool hasCollision;
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

        public Entity(Entity parent, GraphicsDevice graphicsDevice, Texture2D texture2D, Vector2 startPosition, SpriteFont font = null)
        {
            this.sprite = texture2D;
            if (graphicsDevice != null)
            {
                spriteBatch = new SpriteBatch(graphicsDevice);
            }
            gridCoord = CalculateGridCoord(startPosition);
            this.children = new HashSet<Entity>();
            this.updatables = new HashSet<Updatable>();
            this.drawables = new HashSet<Drawable>();
            if (parent != null) {
                this.parent = parent;
                this.parent.AddChild(this);
                this.startPosition = this.currentPosition = startPosition + GetParent().GetPositionWithParent();
            } else
            {
                RootContainer.Instance.AddChild(this);
                this.startPosition = this.currentPosition = startPosition;
            }
            
            this.hasCollision = true;
            //this.startPosition = this.currentPosition = startPosition;

#if GRAPHICS_DEBUG
            pivot = CreateCircle(graphicsDevice, Constants.PIVOT_DIAM);
            this.font = font;
#endif

            Scene.Instance.AddObject(this);
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

            foreach (Drawable child in drawables)
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
                
                pos = currentPosition + RootContainer.Instance.GetRootPosition();
            }

            //spriteBatch.Draw(sprite, pos, sourceRectangle, Color.White, 0f, origin, 1f, SpriteEffects.None, 0);
            if (this is ControllableEntity)
                spriteBatch.Draw(sprite, pos + (new Vector2(-Constants.SPRITE_DRAW_OFFSET, -Constants.SPRITE_DRAW_OFFSET) * Constants.GRID), Color.White);
            else
                spriteBatch.Draw(sprite, pos, Color.White);
            //if (Constants.GRAPHICS_DEBUG)
            //{
#if GRAPHICS_DEBUG
            spriteBatch.Draw(pivot, pos, Color.White);
#endif
            //}

#if GRAPHICS_DEBUG
            if (font != null)
            {
                if (GetParent() != null)
                {
                    spriteBatch.DrawString(font, CalculateGridCoord().X + "\n" + CalculateGridCoord().Y, startPosition + GetParent().GetPositionWithParent(), Color.White);
                } else
                {
                    spriteBatch.DrawString(font, CalculateGridCoord().X + "\n" + CalculateGridCoord().Y, currentPosition + RootContainer.Instance.GetRootPosition(), Color.White);
                }
                
            }
#endif

            spriteBatch.End();

            foreach (Drawable child in drawables)
            {
                child.Draw(gameTime);
            }
        }

        public virtual void PostDraw(GameTime gameTime)
        {

            foreach (Drawable child in drawables)
            {
                child.PostDraw(gameTime);
            }
        }

        public HashSet<Entity> GetAllChildren()
        {
            return children;
        }

        public void AddChild(Entity gameObject)
        {
            children.Add(gameObject);
            if (gameObject is Drawable)
            {
                drawables.Add(gameObject);
            }
            if (gameObject is Updatable)
            {
                updatables.Add((Updatable)gameObject);
            }
        }

        public void RemoveChild(Entity gameObject)
        {
            children.Remove(gameObject);
            if (gameObject is Drawable)
            {
                drawables.Remove(gameObject);
            }
            if (gameObject is Updatable)
            {
                updatables.Remove((Updatable)gameObject);
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

        public Vector2 GetPositionWithParent()
        {
            if (GetParent() != null)
            {
                return currentPosition + GetParent().GetPositionWithParent();
            }
            else
            {
                return currentPosition + RootContainer.Instance.GetRootPosition();
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

        protected HashSet<Updatable> GetUpdatables()
        {
            return updatables;
        }

        protected HashSet<Drawable> GetDrawables()
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
    }
}
