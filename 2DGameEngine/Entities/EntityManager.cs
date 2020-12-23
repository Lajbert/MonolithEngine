using System;
using System.Collections.Generic;
using System.Text;
using _2DGameEngine.Entities;
using _2DGameEngine.Entities.Interfaces;
using Microsoft.Xna.Framework;

namespace _2DGameEngine.Entities
{
    class EntityManager
    {

        private static List<Drawable> drawables = new List<Drawable>();
        private static List<Updatable> updatables = new List<Updatable>();

        //private static readonly EntityManager instance = new EntityManager();

        private EntityManager()
        {
        }

        static EntityManager()
        {
        }

        /*public static EntityManager Instance
        {
            get
            {
                return instance;
            }
        }*/

        public static void AddObject(Object gameObject)
        {
            if (gameObject is Drawable)
            {
                drawables.Add((Drawable)gameObject);
            }
            if (gameObject is Updatable)
            {
                updatables.Add((Updatable)gameObject);
            }
        }

        public static void DrawAll(GameTime gameTime)
        {
            foreach (Drawable o in drawables)
            {
                o.Draw(gameTime);
            }
        }

        public static void UpdateAll(GameTime gameTime)
        {
            foreach (Updatable o in updatables)
            {
                o.Update(gameTime);
            }
        }

    }
}
