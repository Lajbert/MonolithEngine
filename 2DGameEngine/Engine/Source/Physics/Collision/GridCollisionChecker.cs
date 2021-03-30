﻿using MonolithEngine.Engine.Source.Entities;
using MonolithEngine.Engine.Source.Entities.Abstract;
using MonolithEngine.Engine.Source.Entities.Interfaces;
using MonolithEngine.Engine.Source.Level.Collision;
using MonolithEngine.Entities;
using MonolithEngine.Entities.Interfaces;
using MonolithEngine.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonolithEngine.Engine.Source.Physics.Collision
{
    public class GridCollisionChecker
    {
        private Dictionary<Vector2, StaticCollider> objects = new Dictionary<Vector2, StaticCollider>();
        private Dictionary<StaticCollider, Vector2> objectPositions = new Dictionary<StaticCollider, Vector2>();

        private ICollection<Direction> whereToCheck;

        private List<(StaticCollider, Direction)> allCollisionsResult = new List<(StaticCollider, Direction)>();
        List<Direction> tagCollisionResult = new List<Direction>();

        private static readonly List<Direction> basicDirections = new List<Direction>() { Direction.CENTER, Direction.WEST, Direction.EAST, Direction.NORTH, Direction.SOUTH };

        private static readonly GridCollisionChecker instance = new GridCollisionChecker();

        public GridCollisionChecker()
        {

        }

        public void Add(StaticCollider gameObject)
        {
            objectPositions[gameObject] = gameObject.Transform.GridCoordinates;
            objects.Add(gameObject.Transform.GridCoordinates, gameObject);
        }

        public StaticCollider GetColliderAt(Vector2 position)
        {
            if (!objects.ContainsKey(position))
            {
                return null;
            }
            return objects[position];
        }

        public List<Direction> CollidesWithTag(IGameObject entity, string tag, ICollection<Direction> directionsToCheck = null)
        {

            tagCollisionResult.Clear();

            whereToCheck = directionsToCheck ?? basicDirections;

            foreach (Direction direction in whereToCheck)
            {
                if (objects.ContainsKey(GetGridCoord(entity.Transform.GridCoordinates, direction)) && objects[GetGridCoord(entity.Transform.GridCoordinates, direction)].HasTag(tag)
                    && IsExactCollision(entity, direction))
                    //&& !objects[GetGridCoord(gridCoord, direction)].IsBlockedFrom(direction))
                {
                     tagCollisionResult.Add(direction);
                }
            }
            
            return tagCollisionResult;
        }

        public List<(StaticCollider, Direction)> HasGridCollisionAt(IGameObject entity, ICollection<Direction> directionsToCheck = null)
        {

            allCollisionsResult.Clear();

            whereToCheck = directionsToCheck ?? basicDirections;

            foreach (Direction direction in whereToCheck)
            {
                if (objects.ContainsKey(GetGridCoord(entity.Transform.GridCoordinates, direction))
                    && IsExactCollision(entity, direction))
                    //&& !objects[GetGridCoord(gridCoord, direction)].IsBlockedFrom(direction))
                {
                    allCollisionsResult.Add((objects[GetGridCoord(entity.Transform.GridCoordinates, direction)], direction));
                }
            }
            return allCollisionsResult;
            
        }

        private bool IsExactCollision(IGameObject entity, Direction direction)
        {
            //TODO
            /*Logger.Error("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            Logger.Error("!!!!!!!!!!!!!!!!!!!!! FIX THIS, REMOVE THE CASTING AND CREATE A GRID COLLISION COMPONENT FOR WHATEVER IS COLLIDER ON THE GRID!!!!!!!!!!!!!!!!!!!!!!!!!");
            Logger.Error("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");*/
            if (direction == Direction.WEST)
            {
                return entity.Transform.InCellLocation.X <= (entity as Entity).GetCollisionOffset(direction);
            }
            else if (direction == Direction.EAST)
            {
                return entity.Transform.InCellLocation.X >= (entity as Entity).GetCollisionOffset(direction);
            }
            else if (direction == Direction.NORTH)
            {
                return entity.Transform.InCellLocation.Y <= (entity as Entity).GetCollisionOffset(direction);
            }
            else if (direction == Direction.SOUTH)
            {
                return entity.Transform.InCellLocation.Y >= (entity as Entity).GetCollisionOffset(direction);
            }
            else if (direction == Direction.NORTHWEST)
            {
                return IsExactCollision(entity, Direction.NORTH) && IsExactCollision(entity, Direction.WEST);
            }
            else if (direction == Direction.NORTHEAST)
            {
                return IsExactCollision(entity, Direction.NORTH) && IsExactCollision(entity, Direction.EAST);
            }
            else if (direction == Direction.SOUTHWEST)
            {
                return IsExactCollision(entity, Direction.SOUTH) && IsExactCollision(entity, Direction.WEST);
            }
            else if (direction == Direction.SOUTHEAST)
            {
                return IsExactCollision(entity, Direction.SOUTH) && IsExactCollision(entity, Direction.EAST);
            } else if (direction == Direction.CENTER)
            {
                return true;
            }
            throw new Exception("Uknown direction");
        }

        protected Vector2 GetGridCoord(Vector2 gridCoord, Direction direction)
        {
            if (direction == Direction.CENTER) return gridCoord;
            if (direction == Direction.WEST) return GridUtil.GetLeftGrid(gridCoord);
            if (direction == Direction.EAST) return GridUtil.GetRightGrid(gridCoord);
            if (direction == Direction.NORTH) return GridUtil.GetUpperGrid(gridCoord);
            if (direction == Direction.SOUTH) return GridUtil.GetBelowGrid(gridCoord);
            if (direction == Direction.SOUTHEAST) return GridUtil.GetRightBelowGrid(gridCoord);
            if (direction == Direction.SOUTHWEST) return GridUtil.GetLeftBelowGrid(gridCoord);
            if (direction == Direction.NORTHWEST) return GridUtil.GetUpperLeftGrid(gridCoord);
            if (direction == Direction.NORTHEAST) return GridUtil.GetUpperRightGrid(gridCoord);

            throw new Exception("Unknown direction!");
        }

        public bool HasBlockingColliderAt(Vector2 gridCoord, Direction direction)
        {
            Vector2 coord = GetGridCoord(gridCoord, direction);
            return objects.ContainsKey(coord) && objects[coord].BlocksMovementFrom(direction);
        }

        public void Remove(StaticCollider gameObject)
        {
            Vector2 position = objectPositions[gameObject];
            objects.Remove(position);
        }

    }
}
