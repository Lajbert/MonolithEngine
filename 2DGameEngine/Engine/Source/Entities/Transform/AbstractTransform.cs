using GameEngine2D.Engine.Source.Entities.Abstract;
using GameEngine2D.Entities;
using GameEngine2D.Source.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.Engine.Source.Entities.Transform
{
    public abstract class AbstractTransform : ITransform
    {
        protected IGameObject owner;

        public Vector2 GridCoordinates;

        //between 0 and 1: where the object is inside the grid cell
        public Vector2 InCellLocation;

        public abstract Vector2 Velocity { get; set; }

        public float Rotation { get; set; }

        private Vector2 position;

        public Vector2 Position
        {
            get
            {
                if (owner.Parent == null)
                {
                    return position;
                }
                return owner.Parent.Transform.Position + position;
            }
            set
            {
                position = value;
            }
        }

        public float X
        {
            get
            {
                if (owner.Parent == null)
                {
                    return position.X;
                }
                return owner.Parent.Transform.Position.X + position.X;
            }
            set
            {
                position.X = value;
            }
        }

        public float Y
        {
            get
            {
                if (owner.Parent == null)
                {
                    return position.Y;
                }
                return owner.Parent.Transform.Position.Y + position.Y;
            }
            set
            {
                position.Y = value;
            }
        }

        public AbstractTransform(IGameObject owner, Vector2 position = default(Vector2))
        {
            this.owner = owner;
            InCellLocation = new Vector2(0.5f, 1f);
            Position = position;
            GridCoordinates = MathUtil.CalculateGridCoordintes(position);
        }

        public void OverridePositionOffset(Vector2 newPositionOffset)
        {
            this.position = newPositionOffset;
        }

        public void DetachFromParent()
        {
            GridCoordinates = MathUtil.CalculateGridCoordintes(Position);
            position += owner.Transform.Position;
        }
    }
}
