using Microsoft.Xna.Framework;

namespace MonolithEngine
{
    public interface GridUtil
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
    }
}
