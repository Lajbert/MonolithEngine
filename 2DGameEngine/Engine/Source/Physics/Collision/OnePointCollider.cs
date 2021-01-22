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
        private Dictionary<Vector2, Entity> objects = new Dictionary<Vector2, Entity>();
        private Dictionary<Entity, Vector2> objectPositions = new Dictionary<Entity, Vector2>();
        private List<Entity> lowPriorityObjects = new List<Entity>();
        private List<Entity> tmpList = new List<Entity>();

        public OnePointCollider()
        {
        }

        public Entity GetObjectAt(Vector2 position)
        {
            return objects[position];
        }

        public void AddOrUpdate(Entity gameObject)
        {
            if (objectPositions.ContainsKey(gameObject))
            {
                objects.Remove(objectPositions[gameObject]);
            }
            if (objects.ContainsKey(gameObject.GridCoordinates))
            {
                if (objects[gameObject.GridCoordinates] != gameObject && objects[gameObject.GridCoordinates].CollisionPriority < gameObject.CollisionPriority)
                {
                    lowPriorityObjects.Add(objects[gameObject.GridCoordinates]);
                    objects.Remove(gameObject.GridCoordinates);
                }
            }
            objectPositions[gameObject] = gameObject.GridCoordinates;
            objects.Add(gameObject.GridCoordinates, gameObject);
        }

        private void TryRestoreLowPriorityObjects()
        {
            if (lowPriorityObjects.Count == 0)
            {
                return;
            }

            // TODO: review this list copy stuff!
            tmpList.Clear();
            tmpList.AddRange(lowPriorityObjects);
            foreach (Entity e in tmpList)
            {
                if (objects.ContainsKey(e.GridCoordinates))
                {
                    if (objects[e.GridCoordinates].CollisionPriority < e.CollisionPriority)
                    {
                        lowPriorityObjects.Add(objects[e.GridCoordinates]);
                        objects[e.GridCoordinates] = e;
                        lowPriorityObjects.Remove(e);
                    }
                } else
                {
                    objects[e.GridCoordinates] = e;
                }
            }
        }

        public Entity GetColliderAt(Vector2 position)
        {
            return objects[position];
        }

        /*private void RemoveObject(Vector2 position)
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
        }*/

        public List<(Entity, Direction)> HasCollisionAt(Vector2 gridCoord, ICollection<Direction> directionsToCheck = null)
        {
            TryRestoreLowPriorityObjects();

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
            if (direction == Direction.BOTTOMRIGHT) return GridUtil.GetRightBelowGrid(gridCoord);
            if (direction == Direction.BOTTOMLEFT) return GridUtil.GetLeftBelowGrid(gridCoord);

            throw new Exception("Unknown direction!");
        }

        public bool HasObjectAtWithTag(Vector2 gridCoord, string tag)
        {
            TryRestoreLowPriorityObjects();
            return objects.ContainsKey(gridCoord) && objects[gridCoord].HasTag(tag);
        }

        public bool HasBlockingColliderAt(Vector2 gridCoord)
        {
            TryRestoreLowPriorityObjects();
            return objects.ContainsKey(gridCoord) && objects[gridCoord].BlocksMovement;
        }

        public void Remove(Entity gameObject)
        {
            if (gameObject == null)
            {
                return;
            }
            if (objectPositions.ContainsKey(gameObject))
            {
                Vector2 position = objectPositions[gameObject];
                objects.Remove(position);
                /*foreach (Entity child in gameObject.GetAllChildren())
                {
                    Remove(child);
                }*/
            }
            //RemoveObject(gameObject.GridCoordinates);
        }

    }
}
