using Microsoft.Xna.Framework;

namespace MonolithEngine
{
    /// <summary>
    /// Base class to represent entities' transform:
    /// Position, Rotation and Velocity
    /// </summary>
    public abstract class AbstractTransform : ITransform
    {
        protected IGameObject owner;

        internal Vector2 gridCoordinates;

        internal Vector2 GridCoordinates
        {
            get
            {
                if (owner.Parent == null)
                {
                    return gridCoordinates;
                }
                return owner.Parent.Transform.GridCoordinates + gridCoordinates;
            }
            set
            {
                gridCoordinates = value;
            }
        }

        internal float GridCoordinatesX
        {
            get
            {
                if (owner.Parent == null)
                {
                    return gridCoordinates.X;
                }
                return owner.Parent.Transform.GridCoordinates.X + gridCoordinates.X;
            }
            set
            {
                gridCoordinates.X = value;
            }
        }

        internal float GridCoordinatesY
        {
            get
            {
                if (owner.Parent == null)
                {
                    return gridCoordinates.Y;
                }
                return owner.Parent.Transform.GridCoordinates.Y + gridCoordinates.Y;
            }
            set
            {
                gridCoordinates.Y = value;
            }
        }

        //between 0 and 1: where the object is inside the grid cell
        internal Vector2 InCellLocation;

        public abstract Vector2 Velocity { get; set; }

        public abstract float VelocityX { get; set; }

        public abstract float VelocityY { get; set; }

        public float Rotation { get; set; }

        internal Vector2 PositionWithoutParent;

        public Vector2 Position
        {
            get
            {
                if (owner.Parent == null)
                {
                    return PositionWithoutParent;
                }
                return owner.Parent.Transform.Position + PositionWithoutParent;
            }
            set
            {
                PositionWithoutParent = value;
                Reposition(value);
            }
        }

        public float X
        {
            get
            {
                if (owner.Parent == null)
                {
                    return PositionWithoutParent.X;
                }
                return owner.Parent.Transform.Position.X + PositionWithoutParent.X;
            }
            internal set
            {
                PositionWithoutParent.X = value;
                InCellLocation.X = MathUtil.CalculateInCellLocation(PositionWithoutParent).X;
                gridCoordinates.X = (int)(PositionWithoutParent.X / Config.GRID);
            }
        }

        public float Y
        {
            get
            {
                if (owner.Parent == null)
                {
                    return PositionWithoutParent.Y;
                }
                return owner.Parent.Transform.Position.Y + PositionWithoutParent.Y;
            }
            internal set
            {
                PositionWithoutParent.Y = value;
                InCellLocation.Y = MathUtil.CalculateInCellLocation(PositionWithoutParent).Y;
                gridCoordinates.Y = (int)(PositionWithoutParent.Y / Config.GRID);
            }
        }

        public AbstractTransform(IGameObject owner, Vector2 position = default)
        {
            this.owner = owner;
            Position = position;
        }

        public void OverridePositionOffset(Vector2 newPositionOffset)
        {
            PositionWithoutParent = newPositionOffset;
        }

        public void DetachFromParent()
        {
            PositionWithoutParent = owner.Transform.Position;
            Reposition(PositionWithoutParent);
        }

        private void Reposition(Vector2 position)
        {
            GridCoordinates = MathUtil.CalculateGridCoordintes(position);
            InCellLocation = MathUtil.CalculateInCellLocation(position);
        }
    }
}
