﻿using System;
using System.Collections.Generic;
using System.Text;
using _2DGameEngine.Entities.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using _2DGameEngine.Global;
using _2DGameEngine.Util;
using _2DGameEngine.Level;

namespace _2DGameEngine.Entities
{
    class Entity : GameObject, Drawable, HasParent, HasChildren, Collider
    {

        protected Vector2 position;
        protected Texture2D sprite;
        protected SpriteBatch spriteBatch;
        private HashSet<HasParent> children;
        private HashSet<Updatable> updatables;
        private HashSet<Drawable> drawables;
        private HasChildren parent;
        private bool hasCollision;

        protected MyLevel level;

        protected SpriteFont font;

        public Entity(MyLevel level, HasChildren parent, GraphicsDevice graphicsDevice, Texture2D texture2D, Vector2 startPosition, SpriteFont font = null)
        {
            if (level == null || parent == null)
            {
                throw new Exception("Parent object or level not set!");
            }
            this.sprite = texture2D;
            if (graphicsDevice != null)
            {
                spriteBatch = new SpriteBatch(graphicsDevice);
            }
            this.level = level;
            this.children = new HashSet<HasParent>();
            this.updatables = new HashSet<Updatable>();
            this.drawables = new HashSet<Drawable>();
            this.parent = parent;
            this.parent.AddChild(this);
            this.hasCollision = true;
            this.position = startPosition;
            this.font = font;

            level.AddObject(this);
        }



        public virtual void Draw(GameTime gameTime)
        {
            //position += GetParent().GetPosition();
            spriteBatch.Begin();

            spriteBatch.Draw(sprite, position + GetParent().GetPosition(), Color.White);

            if (font != null)
            {
                spriteBatch.DrawString(font, GetGridCoord().X + "\n" + GetGridCoord().Y, position, Color.White);
            }

            spriteBatch.End();
        }

        public HashSet<HasParent> GetAllChildren()
        {
            return children;
        }

        public void AddChild(HasParent gameObject)
        {
            children.Add(gameObject);
            if (gameObject is Drawable)
            {
                drawables.Add((Drawable)gameObject);
            }
            if (gameObject is Updatable)
            {
                updatables.Add((Updatable)gameObject);
            }
        }

        public void RemoveChild(HasParent gameObject)
        {
            children.Remove(gameObject);
            if (gameObject is Drawable)
            {
                drawables.Remove((Drawable)gameObject);
            }
            if (gameObject is Updatable)
            {
                updatables.Remove((Updatable)gameObject);
            }
        }

        public HasChildren GetParent()
        {
            return parent;
        }

        public override void Destroy()
        {
            parent.RemoveChild(this);
            if (!children.Any())
            {
                foreach (HasParent o in children) {
                    if (o != null) {
                        ((GameObject)o).Destroy();
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


        protected Vector2 GetGridCoord()
        {
            return GetGridCoord(position);
        }

        protected Vector2 GetGridCoord(Vector2 position)
        {
            return new Vector2((int)Math.Floor(position.X / Constants.GRID), (int)Math.Floor(position.Y / Constants.GRID));
        }
    }
}