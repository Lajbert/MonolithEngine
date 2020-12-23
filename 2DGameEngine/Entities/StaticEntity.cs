using System;
using System.Collections.Generic;
using System.Text;
using _2DGameEngine.Entities.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _2DGameEngine.Entities
{
    class StaticEntity : Drawable, HasParent, HasChildren
    {

        protected Vector2 position;
        protected SpriteBatch spriteBatch;
        private List<HasParent> children;
        private List<Updatable> updatables;
        private List<Drawable> drawables;
        private HasChildren parent;

        public StaticEntity(HasChildren parent, GraphicsDevice graphicsDevice, Texture2D texture2D, Vector2 startPosition, float speed = 0.5f)
        {
            this.sprite = texture2D;
            this.position = startPosition;
            spriteBatch = new SpriteBatch(graphicsDevice);
            this.children = new List<HasParent>();
            this.updatables = new List<Updatable>();
            this.drawables = new List<Drawable>();
            this.parent = parent;
            this.parent.AddChild(this);
        }

        public Texture2D sprite { get ; set; }

        public virtual void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(sprite, position, Color.White);
            spriteBatch.End();
        }

        public List<HasParent> GetAllChildren()
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

        public HasChildren GetParent()
        {
            return parent;
        }
    }
}
