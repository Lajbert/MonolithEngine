using _2DGameEngine.Entities;
using _2DGameEngine.Entities.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace _2DGameEngine.src.Layer
{
    class GraphicsLayer
    {
        private Dictionary<Vector2, Entity> objects;
        private float scrollSpeedModifier;
        private bool lockY;

        public GraphicsLayer(float scrollSpeedModifier = 1f, bool lockY = false)
        {
            this.objects = new Dictionary<Vector2, Entity>();
            this.scrollSpeedModifier = scrollSpeedModifier;
            this.lockY = lockY;
        }

        public void AddObject(Entity gameObject)
        {
            objects.Add(gameObject.GetGridCoord(), gameObject);
        }

        public void RemoveObject(Vector2 position)
        {
            Entity e = objects[position];
            objects.Remove(position);
            foreach (Entity child in e.GetAllChildren()) {
                RemoveObject(e.GetPosition());
            }
        }

        public bool HasObjectAt(Vector2 position)
        {
            return objects.ContainsKey(position);
        }

        public Vector2 GetPosition()
        {
            if (lockY)
            {
                return new Vector2(RootContainer.Instance.GetRootPosition().X * scrollSpeedModifier, RootContainer.Instance.GetRootPosition().Y);
            }
            return RootContainer.Instance.GetRootPosition() * scrollSpeedModifier;
        }


    }
}
