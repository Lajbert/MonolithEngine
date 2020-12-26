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

namespace _2DGameEngine.Entities
{
    class Entity : GameObject, Drawable, HasParent, HasChildren, Collider
    {

        protected Vector2 position;
        protected Texture2D sprite;
        protected SpriteBatch spriteBatch;
        private HashSet<Entity> children;
        private HashSet<Updatable> updatables;
        private HashSet<Drawable> drawables;
        private Entity parent;
        private bool hasCollision;

        //grid coordinates
        //private float cx = 0f;
        //private float cy = 0f;
        protected Vector2 gridCoord = Vector2.Zero;

        protected SpriteFont font;

        public Entity(Entity parent, GraphicsDevice graphicsDevice, Texture2D texture2D, Vector2 startPosition, SpriteFont font = null)
        {
            this.sprite = texture2D;
            if (graphicsDevice != null)
            {
                spriteBatch = new SpriteBatch(graphicsDevice);
            }
            this.children = new HashSet<Entity>();
            this.updatables = new HashSet<Updatable>();
            this.drawables = new HashSet<Drawable>();
            if (parent != null) {
                this.parent = parent;
                this.parent.AddChild(this);
            } else
            {
                RootContainer.Instance.AddChild(this);
            }
            
            this.hasCollision = true;
            this.position = startPosition;
            this.font = font;

            Scene.Instance.AddObject(this);
        }


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
            if (GetParent() != null)
            {
                spriteBatch.Draw(sprite, position + GetParent().GetPosition(), Color.White);
            } else
            {
                spriteBatch.Draw(sprite, position + RootContainer.Instance.GetRootPosition(), Color.White);
            }
            

            if (font != null)
            {
                if (GetParent() != null)
                {
                    spriteBatch.DrawString(font, CalculateGridCoord().X + "\n" + CalculateGridCoord().Y, position + GetParent().GetPosition(), Color.White);
                } else
                {
                    spriteBatch.DrawString(font, CalculateGridCoord().X + "\n" + CalculateGridCoord().Y, position + RootContainer.Instance.GetRootPosition(), Color.White);
                }
                
            }

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
            return this.position;
        }

        public Vector2 GetCenter()
        {
            return position;
        }

        protected Vector2 CalculateGridCoord()
        {
            return CalculateGridCoord(position);
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
