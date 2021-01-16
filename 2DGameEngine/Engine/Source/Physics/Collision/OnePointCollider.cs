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

        public OnePointCollider(float scrollSpeedModifier = 1f, bool lockY = false)
        {
            objects = new Dictionary<Vector2, Entity>();
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
                RemoveObject(child.GridCoordinates);
            }
        }

        public bool HasObjectAtWithTag(Vector2 gridCoord, string tag)
        {
            return objects.ContainsKey(gridCoord) && objects[gridCoord].HasTag(tag);
        }

        public bool HasBlockingColliderAt(Vector2 gridCoord)
        {
            return objects.ContainsKey(gridCoord) && objects[gridCoord].BlocksMovement;
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
