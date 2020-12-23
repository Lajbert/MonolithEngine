﻿using System;
using System.Collections.Generic;
using System.Text;
using _2DGameEngine.Entities.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using _2DGameEngine.Global;

namespace _2DGameEngine.Entities
{
    class Entity : GameObject, Drawable, HasParent, HasChildren, Collider
    {

        protected Vector2 position;
        private Texture2D sprite;
        protected SpriteBatch spriteBatch;
        private HashSet<HasParent> children;
        private HashSet<Updatable> updatables;
        private HashSet<Drawable> drawables;
        private HasChildren parent;
        private bool hasCollision;

        private SpriteFont font;

        //between 0 and 1: where the object is inside the grid cell
        //private float xr = 0.5f;
        //private float yr = 1.0f;
        protected Vector2 inCellLocation = new Vector2(0.5f, 1.0f);

        //grid coordinates
        //private float cx = 0f;
        //private float cy = 0f;
        protected Vector2 gridCoord = Vector2.Zero;

        public Entity(HasChildren parent, GraphicsDevice graphicsDevice, Texture2D texture2D, Vector2 startPosition, SpriteFont font = null)
        {
            //SetID();
            this.sprite = texture2D;
            this.position = startPosition;
            spriteBatch = new SpriteBatch(graphicsDevice);
            this.children = new HashSet<HasParent>();
            this.updatables = new HashSet<Updatable>();
            this.drawables = new HashSet<Drawable>();
            this.parent = parent;
            this.parent.AddChild(this);
            this.hasCollision = true;
            SetPosition(startPosition);
            this.font = font;
        }



        public virtual void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(sprite, position, Color.White);
            if (font != null)
            {
                spriteBatch.DrawString(font, gridCoord.X + "\n" + gridCoord.Y, position, Color.White);
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

        public void SetPosition(Vector2 positon)
        {
            gridCoord = positon / Constants.GRID;
            inCellLocation = new Vector2(0f, 0f);
        }
    }
}