using _2DGameEngine.Entities;
using _2DGameEngine.Entities.Interfaces;
using _2DGameEngine.Global;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace _2DGameEngine.src
{
    class Scene
    {

        private Dictionary<Vector2, Collider> colliders;
        private Scene() {
            this.colliders = new Dictionary<Vector2, Collider>();
        }
        static Scene() { }

        private static readonly Scene instance = new Scene();

        public bool HasColliderAt(Vector2 location)
        {
            return colliders.ContainsKey(location);
        }

        public void AddObject(GameObject gameObject)
        {
            if (gameObject is Collider)
            {
                Collider c = (Collider)gameObject;
                Vector2 gridLocation = new Vector2((int)Math.Round(c.GetPosition().X / Constants.GRID), (int)Math.Round(c.GetPosition().Y / Constants.GRID));
                colliders.Add(gridLocation, c);
            }
        }

        public static Scene Instance
        {
            get
            {
                return instance;
            }
        }
    }
}
