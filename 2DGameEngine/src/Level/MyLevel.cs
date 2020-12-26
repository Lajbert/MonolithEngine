using _2DGameEngine.Entities.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using _2DGameEngine.Entities;
using System.Text;
using _2DGameEngine.Global;
using _2DGameEngine.Util;

namespace _2DGameEngine.Level
{
    class MyLevel
    {

        private Dictionary<Vector2, Collider> colliders;

        public MyLevel()
        {
            colliders = new Dictionary<Vector2, Collider>();
        }

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
    }
}
