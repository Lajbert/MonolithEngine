using GameEngine2D.Entities;
using GameEngine2D.Entities.Interfaces;
using GameEngine2D.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.Source.Layer
{
    public class OnePointCollider
    {
        private Dictionary<Vector2, Entity> objects;
        private float scrollSpeedModifier;
        private bool lockY;

        public OnePointCollider(float scrollSpeedModifier = 1f, bool lockY = false)
        {
            this.objects = new Dictionary<Vector2, Entity>();
            this.scrollSpeedModifier = scrollSpeedModifier;
            this.lockY = lockY;
        }

        public Entity GetObjectAt(Vector2 position)
        {
            return objects[position];
        }

        public void AddObject(Entity gameObject)
        {
            objects.Add(gameObject.GridCoordinates, gameObject);
        }

        public Entity GetColliderAt(Vector2 position)
        {
            return objects[position];
        }

        private void RemoveObject(Vector2 position)
        {
            if (!objects.ContainsKey(position))
            {
                return;
            }
            Entity e = objects[position];
            objects.Remove(position);
            foreach (Entity child in e.GetAllChildren()) {
                RemoveObject(e.GridCoordinates);
            }
        }

        public bool HasColliderAt(Vector2 position)
        {
            return objects.ContainsKey(position);
        }

        public Vector2 GetPosition()
        {
            if (lockY)
            {
                return new Vector2(RootContainer.Instance.Position.X * scrollSpeedModifier, RootContainer.Instance.Position.Y);
            }
            return RootContainer.Instance.Position * scrollSpeedModifier;
        }

        public IEnumerable<Entity> GetAll()
        {
            return objects.Values;
        }

        public void Remove(Entity gameObject)
        {
            RemoveObject(gameObject.GridCoordinates);
        }

    }
}
