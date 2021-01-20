using GameEngine2D.Engine.Source.Entities;
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

        public List<(Entity, Direction)> HasCollisionAt(Vector2 gridCoord, List<Direction> directionsToCheck = null)
        {
            List<(Entity, Direction)> result = new List<(Entity, Direction)>();
            if (directionsToCheck == null)
            {
                if (objects.ContainsKey(gridCoord))
                {
                    result.Add((objects[gridCoord], Direction.CENTER));
                }
            } else
            {
                foreach (Direction direction in directionsToCheck)
                {
                    if (objects.ContainsKey(GetGridCoord(gridCoord, direction)))
                    {
                        result.Add((objects[GetGridCoord(gridCoord, direction)], direction));
                    }
                }
            }

            return result;
            
        }

        private Vector2 GetGridCoord(Vector2 gridCoord, Direction direction)
        {
            if (direction == Direction.CENTER) return gridCoord;
            if (direction == Direction.LEFT) return GridUtil.GetLeftGrid(gridCoord);
            if (direction == Direction.RIGHT) return GridUtil.GetRightGrid(gridCoord);
            if (direction == Direction.UP) return GridUtil.GetUpperGrid(gridCoord);
            if (direction == Direction.DOWN) return GridUtil.GetBelowGrid(gridCoord);

            throw new Exception("Unknown direction!");
        }

        public bool HasObjectAtWithTag(Vector2 gridCoord, string tag)
        {
            return objects.ContainsKey(gridCoord) && objects[gridCoord].HasTag(tag);
        }

        public bool HasColliderAt(Vector2 gridCoord)
        {
            return objects.ContainsKey(gridCoord);
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
