using System;
using System.Collections.Generic;
using System.Text;
using _2DGameEngine.Entities;
using _2DGameEngine.Entities.Interfaces;
using _2DGameEngine.src.Layer;
using Microsoft.Xna.Framework;

namespace _2DGameEngine.Entities
{
    class RootContainer : GameObject, IHasChildren
    {

        private HashSet<Entity> children = new HashSet<Entity>();
        private List<Interfaces.IDrawable> drawables;
        private List<IUpdatable> updatables;
        private List<GraphicsLayer> layers;
        private Vector2 position = Vector2.Zero;

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
            layers = new List<GraphicsLayer>();
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
        }

        public static RootContainer Instance
        {
            get
            {
                return instance;
            }
        }

        public Vector2 GetRootPosition()
        {
            return position;
        }

        public void SetPosition(Vector2 newPosition)
        {
            position = newPosition;
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
                o.PreDraw(gameTime);
            }
        }

        public void UpdateAll(GameTime gameTime)
        {
            foreach (IUpdatable o in updatables)
            {
                o.PreUpdate(gameTime);
                o.Update(gameTime);
                o.PreUpdate(gameTime);
            }
        }
    }
}
