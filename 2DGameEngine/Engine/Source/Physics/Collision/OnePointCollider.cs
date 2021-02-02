using GameEngine2D.Engine.Source.Entities;
using GameEngine2D.Engine.Source.Entities.Interfaces;
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
        private Dictionary<Vector2, IGridCollider> objects = new Dictionary<Vector2, IGridCollider>();
        private Dictionary<IGridCollider, Vector2> objectPositions = new Dictionary<IGridCollider, Vector2>();
        private List<IGridCollider> lowPriorityObjects = new List<IGridCollider>();
        private List<IGridCollider> tmpList = new List<IGridCollider>();

        private Dictionary<string, HashSet<Direction>> directionsForTags = new Dictionary<string, HashSet<Direction>>();

        private ICollection<Direction> whereToCheck;

        private List<(IGridCollider, Direction)> allCollisionsResult = new List<(IGridCollider, Direction)>();
        List<Direction> tagCollisionResult = new List<Direction>();

        private static readonly List<Direction> basicDirections = new List<Direction>() { Direction.CENTER, Direction.LEFT, Direction.RIGHT, Direction.UP, Direction.DOWN };

        public OnePointCollider()
        {
        }

        public IGridCollider GetObjectAt(Vector2 position, Direction direction)
        {
            if (!objects.ContainsKey(GetGridCoord(position, direction)))
            {
                return null;
            } 
            return objects[GetGridCoord(position, direction)];
        }

        public void AddOrUpdate(IGridCollider gameObject)
        {
            if (objectPositions.ContainsKey(gameObject))
            {
                objects.Remove(objectPositions[gameObject]);
            }
            if (objects.ContainsKey(gameObject.GetGridCoord()))
            {
                if (objects[gameObject.GetGridCoord()] != gameObject && objects[gameObject.GetGridCoord()].GridCollisionPriority <= gameObject.GridCollisionPriority)
                {
                    lowPriorityObjects.Add(objects[gameObject.GetGridCoord()]);
                    objects.Remove(gameObject.GetGridCoord());
                }
            }
            objectPositions[gameObject] = gameObject.GetGridCoord();
            objects.Add(gameObject.GetGridCoord(), gameObject);
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
            foreach (IGridCollider e in tmpList)
            {
                if (objects.ContainsKey(e.GetGridCoord()))
                {
                    if (objects[e.GetGridCoord()].GridCollisionPriority <= e.GridCollisionPriority)
                    {
                        lowPriorityObjects.Add(objects[e.GetGridCoord()]);
                        objects[e.GetGridCoord()] = e;
                        lowPriorityObjects.Remove(e);
                    }
                } else
                {
                    objects[e.GetGridCoord()] = e;
                }
            }
        }

        public IGridCollider GetColliderAt(Vector2 position)
        {
            if (!objects.ContainsKey(position))
            {
                return null;
            }
            return objects[position];
        }

        public List<Direction> CollidesWithTag(IGridCollider entity, string tag, ICollection<Direction> directionsToCheck = null)
        {
            TryRestoreLowPriorityObjects();

            tagCollisionResult.Clear();

            whereToCheck = directionsToCheck ?? basicDirections;

            foreach (Direction direction in whereToCheck)
            {
                if (objects.ContainsKey(GetGridCoord(entity.GetGridCoord(), direction)) && objects[GetGridCoord(entity.GetGridCoord(), direction)].HasTag(tag)
                    && IsExactCollision(entity, direction))
                    //&& !objects[GetGridCoord(gridCoord, direction)].IsBlockedFrom(direction))
                {
                    if  (!directionsForTags.ContainsKey(tag) || (directionsForTags.ContainsKey(tag) && directionsForTags[tag].Contains(direction))) {
                        tagCollisionResult.Add(direction);
                    }
                }
            }
            

            return tagCollisionResult;
        }

        public List<(IGridCollider, Direction)> HasGridCollisionAt(IGridCollider entity, ICollection<Direction> directionsToCheck = null)
        {
            TryRestoreLowPriorityObjects();

            allCollisionsResult.Clear();

            whereToCheck = directionsToCheck ?? basicDirections;

            foreach (Direction direction in whereToCheck)
            {
                if (objects.ContainsKey(GetGridCoord(entity.GetGridCoord(), direction))
                    && IsExactCollision(entity, direction))
                    //&& !objects[GetGridCoord(gridCoord, direction)].IsBlockedFrom(direction))
                {
                    if (directionsForTags.Count != 0)
                    {
                        foreach (string tag in directionsForTags.Keys)
                        {
                            if (!objects[GetGridCoord(entity.GetGridCoord(), direction)].HasTag(tag) || (objects[GetGridCoord(entity.GetGridCoord(), direction)].HasTag(tag) && directionsForTags[tag].Contains(direction)))
                            {
                                allCollisionsResult.Add((objects[GetGridCoord(entity.GetGridCoord(), direction)], direction));
                            }
                        }
                    }
                }
            }
            return allCollisionsResult;
            
        }

        private bool IsExactCollision(IGridCollider entity, Direction direction)
        {
            if (direction == Direction.LEFT)
            {
                return entity.GetInCellLocation().X <= entity.GetCollisionOffset(direction);
            }
            else if (direction == Direction.RIGHT)
            {
                return entity.GetInCellLocation().X >= entity.GetCollisionOffset(direction);
            }
            else if (direction == Direction.UP)
            {
                return entity.GetInCellLocation().Y <= entity.GetCollisionOffset(direction);
            }
            else if (direction == Direction.DOWN)
            {
                return entity.GetInCellLocation().Y >= entity.GetCollisionOffset(direction);
            }
            else if (direction == Direction.TOPLEFT)
            {
                return IsExactCollision(entity, Direction.UP) && IsExactCollision(entity, Direction.LEFT);
            }
            else if (direction == Direction.TOPRIGHT)
            {
                return IsExactCollision(entity, Direction.UP) && IsExactCollision(entity, Direction.RIGHT);
            }
            else if (direction == Direction.BOTTOMLEFT)
            {
                return IsExactCollision(entity, Direction.DOWN) && IsExactCollision(entity, Direction.LEFT);
            }
            else if (direction == Direction.BOTTOMRIGHT)
            {
                return IsExactCollision(entity, Direction.DOWN) && IsExactCollision(entity, Direction.RIGHT);
            } else if (direction == Direction.CENTER)
            {
                return true;
            }
            throw new Exception("Uknown direction");
        }

        protected Vector2 GetGridCoord(Vector2 gridCoord, Direction direction)
        {
            if (direction == Direction.CENTER) return gridCoord;
            if (direction == Direction.LEFT) return GridUtil.GetLeftGrid(gridCoord);
            if (direction == Direction.RIGHT) return GridUtil.GetRightGrid(gridCoord);
            if (direction == Direction.UP) return GridUtil.GetUpperGrid(gridCoord);
            if (direction == Direction.DOWN) return GridUtil.GetBelowGrid(gridCoord);
            if (direction == Direction.BOTTOMRIGHT) return GridUtil.GetRightBelowGrid(gridCoord);
            if (direction == Direction.BOTTOMLEFT) return GridUtil.GetLeftBelowGrid(gridCoord);
            if (direction == Direction.TOPLEFT) return GridUtil.GetUpperLeftGrid(gridCoord);
            if (direction == Direction.TOPRIGHT) return GridUtil.GetUpperRightGrid(gridCoord);

            throw new Exception("Unknown direction!");
        }

        public bool HasBlockingColliderAt(Vector2 gridCoord, Direction direction)
        {
            TryRestoreLowPriorityObjects();
            Vector2 coord = GetGridCoord(gridCoord, direction);
            return objects.ContainsKey(coord) && objects[coord].BlocksMovementFrom(direction);
        }

        public void Remove(IGridCollider gameObject)
        {
            if (gameObject == null)
            {
                return;
            }
            if (objectPositions.ContainsKey(gameObject))
            {
                Vector2 position = objectPositions[gameObject];
                objects.Remove(position);
                /*foreach (IGridCollider child in gameObject.GetAllChildren())
                {
                    Remove(child);
                }*/
            }
            //RemoveObject(gameObject.GetGridCoord());
        }

        public void RestrictDirectionsForTag(string tag, ICollection<Direction> directions)
        {
            directionsForTags.Add(tag, new HashSet<Direction>(directions));
        }

    }
}
