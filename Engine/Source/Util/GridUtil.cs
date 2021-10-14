using Microsoft.Xna.Framework;
using System;

namespace MonolithEngine
{
    public class GridUtil
    {
        public static Vector2 GetRightGrid(IGameObject entity)
        {
            return new Vector2(entity.Transform.GridCoordinates.X + 1, entity.Transform.GridCoordinates.Y);
        }

        public static Vector2 GetLeftGrid(IGameObject entity)
        {
            return new Vector2(entity.Transform.GridCoordinates.X - 1, entity.Transform.GridCoordinates.Y);
        }

        public static Vector2 GetUpperGrid(IGameObject entity)
        {
            return new Vector2(entity.Transform.GridCoordinates.X, entity.Transform.GridCoordinates.Y - 1);
        }

        public static Vector2 GetUpperRightGrid(IGameObject entity)
        {
            return new Vector2(entity.Transform.GridCoordinates.X + 1, entity.Transform.GridCoordinates.Y - 1);
        }

        public static Vector2 GetUpperLeftGrid(IGameObject entity)
        {
            return new Vector2(entity.Transform.GridCoordinates.X - 1, entity.Transform.GridCoordinates.Y - 1);
        }

        public static Vector2 GetBelowGrid(IGameObject entity)
        {
            return new Vector2(entity.Transform.GridCoordinates.X, entity.Transform.GridCoordinates.Y + 1);
        }

        public static Vector2 GetRightBelowGrid(IGameObject entity)
        {
            return new Vector2(entity.Transform.GridCoordinates.X + 1, entity.Transform.GridCoordinates.Y + 1);
        }

        public static Vector2 GetLeftBelowGrid(IGameObject entity)
        {
            return new Vector2(entity.Transform.GridCoordinates.X - 1, entity.Transform.GridCoordinates.Y + 1);
        }

        public static Vector2 GetGridCoord(IGameObject entity, Direction direction)
        {
            if (direction == Direction.CENTER) return entity.Transform.GridCoordinates;
            if (direction == Direction.WEST) return GridUtil.GetLeftGrid(entity);
            if (direction == Direction.EAST) return GridUtil.GetRightGrid(entity);
            if (direction == Direction.NORTH) return GridUtil.GetUpperGrid(entity);
            if (direction == Direction.SOUTH) return GridUtil.GetBelowGrid(entity);
            if (direction == Direction.SOUTHEAST) return GridUtil.GetRightBelowGrid(entity);
            if (direction == Direction.SOUTHWEST) return GridUtil.GetLeftBelowGrid(entity);
            if (direction == Direction.NORTHWEST) return GridUtil.GetUpperLeftGrid(entity);
            if (direction == Direction.NORTHEAST) return GridUtil.GetUpperRightGrid(entity);

            throw new Exception("Unknown direction!");
        }
    }
}
