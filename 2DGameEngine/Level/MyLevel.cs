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
        private List<Drawable> drawables;
        private List<Updatable> updatables;

        public MyLevel()
        {
            colliders = new Dictionary<Vector2, Collider>();
            drawables = new List<Drawable>();
            updatables = new List<Updatable>();
    }

        public bool HasColliderAt(Vector2 location)
        {
            if(colliders.ContainsKey(location))
            {
                Logger.Log("COLLISION LOOKUP AT: " + location);
            }
            return colliders.ContainsKey(location);
        }

        public void AddObject(GameObject gameObject)
        {
            if (gameObject is Drawable)
            {
                drawables.Add((Drawable)gameObject);
            }
            if (gameObject is Updatable)
            {
                updatables.Add((Updatable)gameObject);
            }
            if (gameObject is Collider)
            {
                Collider c = (Collider)gameObject;
                Vector2 gridLocation = new Vector2((int)Math.Floor(c.GetPosition().X / Constants.GRID), (int)Math.Floor(c.GetPosition().Y / Constants.GRID));
                Logger.Log("COLLIDER ADDED AT: " + gridLocation);
                colliders.Add(gridLocation, c);
            }
        }

        public void DrawAll(GameTime gameTime)
        {
            foreach (Drawable o in drawables)
            {
                o.Draw(gameTime);
            }
        }

        public void UpdateAll(GameTime gameTime)
        {
            foreach (Updatable o in updatables)
            {
                o.Update(gameTime);
            }
        }
    }
}
