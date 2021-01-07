using System;
using System.Collections.Generic;
using System.Text;
using GameEngine2D.Entities;
using GameEngine2D.Entities.Interfaces;
using GameEngine2D.Global;
using GameEngine2D.Source.Layer;
using Microsoft.Xna.Framework;

namespace GameEngine2D.Entities
{
    public class RootContainer : GameObject
    {

        private HashSet<Entity> children = new HashSet<Entity>();
        private List<Interfaces.IDrawable> drawables;
        private List<IUpdatable> updatables;
        private Vector2 position = Vector2.Zero;

        public Vector2 Position
        {
            get => position;
            set => position = value;
        }

        public float X {
            set { position.X = value; }
            get { return position.X; } 
        }

        public float Y
        {
            set { position.Y = value; }
            get { return position.Y; }
        }

        private static readonly RootContainer instance = new RootContainer();

        private RootContainer()
        {
            drawables = new List<Interfaces.IDrawable>();
            updatables = new List<IUpdatable>();
        }
        static RootContainer()
        {
        }

        public void AddChild(Entity gameObject)
        {
            if (gameObject is Interfaces.IDrawable)
            {
                drawables.Add((Interfaces.IDrawable)gameObject);
            }
            if (gameObject is IUpdatable)
            {
                updatables.Add((IUpdatable)gameObject);
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
            updatables.Remove(gameObject);
            drawables.Remove(gameObject);
        }

        public static RootContainer Instance
        {
            get
            {
                return instance;
            }
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
            foreach (Interfaces.IDrawable o in drawables)
            {
                o.PreDraw(gameTime);
                o.Draw(gameTime);
                o.PostDraw(gameTime);
            }
        }

        public void UpdateAll(GameTime gameTime)
        {
            foreach (IUpdatable o in new List<IUpdatable>(updatables))
            {
                o.PreUpdate(gameTime);
                o.Update(gameTime);
                o.PostUpdate(gameTime);
            }
        }
    }
}
