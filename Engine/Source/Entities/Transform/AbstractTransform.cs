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

        public Vector2 GridCoordinates;

        //between 0 and 1: where the object is inside the grid cell
        public Vector2 InCellLocation;

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
            set
            {
                PositionWithoutParent.X = value;
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
            set
            {
                PositionWithoutParent.Y = value;
            }
        }

        public AbstractTransform(IGameObject owner, Vector2 position = default)
        {
            this.owner = owner;
            InCellLocation = MathUtil.CalculateInCellLocation(position);
            Position = position;
            GridCoordinates = MathUtil.CalculateGridCoordintes(position);
        }

        public void OverridePositionOffset(Vector2 newPositionOffset)
        {
            this.PositionWithoutParent = newPositionOffset;
        }

        public void DetachFromParent()
        {
            GridCoordinates = MathUtil.CalculateGridCoordintes(Position);
            PositionWithoutParent = owner.Transform.Position;
        }
    }
}
