using System;
using System.Collections.Generic;
using System.Text;
using _2DGameEngine.Entities;
using _2DGameEngine.Entities.Interfaces;
using Microsoft.Xna.Framework;

namespace _2DGameEngine.Entities
{
    class RootContainer : GameObject, HasChildren
    {

        private HashSet<Entity> children = new HashSet<Entity>();
        private List<Drawable> drawables;
        private List<Updatable> updatables;

        private static readonly RootContainer instance = new RootContainer();

        private RootContainer()
        {
            drawables = new List<Drawable>();
            updatables = new List<Updatable>();
        }
        static RootContainer()
        {
        }

        public void AddChild(Entity gameObject)
        {
            if (gameObject is Drawable)
            {
                drawables.Add((Drawable)gameObject);
            }
            if (gameObject is Updatable)
            {
                updatables.Add((Updatable)gameObject);
            }
            children.Add(gameObject);
        }

        public HashSet<Entity> GetAllChildren()
        {
            return children;
        }

        public void RemoveChild(Entity gameObject)
        {
            children.Remove(gameObject);
        }

        public static RootContainer Instance
        {
            get
            {
                return instance;
            }
        }

        public Vector2 GetPosition()
        {
            return Vector2.Zero;
        }

        public override void Destroy()
        {
            foreach(Entity root in children)
            {
                root.Destroy();
            }
        }

        public void DrawAll(GameTime gameTime)
        {
            foreach (Drawable o in drawables)
            {
                o.PreDraw(gameTime);
                o.Draw(gameTime);
                o.PreDraw(gameTime);
            }
        }

        public void UpdateAll(GameTime gameTime)
        {
            foreach (Updatable o in updatables)
            {
                o.PreUpdate(gameTime);
                o.Update(gameTime);
                o.PreUpdate(gameTime);
            }
        }
    }
}
