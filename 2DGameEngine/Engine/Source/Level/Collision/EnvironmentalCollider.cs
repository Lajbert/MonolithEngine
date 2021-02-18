using GameEngine2D.Engine.Source.Entities;
using GameEngine2D.Engine.Source.Physics.Collision;
using GameEngine2D.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.Engine.Source.Level.Collision
{
    public class EnvironmentalCollider : GameObject
    {

        public EnvironmentalCollider(Vector2 gridPosition) : base(null)
        {
            Transform = new StaticTransform(this);
            Transform.GridCoordinates = gridPosition;
            GridCollisionChecker.Instance.Add(this);
        }

        private HashSet<Direction> blockedFrom = new HashSet<Direction>();

        private bool blocksMovement = true;

        public bool BlocksMovement
        {
            get => blocksMovement;
            set
            {
                blocksMovement = value;
                if (value)
                {
                    GridCollisionChecker.Instance.Add(this);
                }
            }
        }

        public override void Destroy()
        {
            throw new NotImplementedException();
        }

        public void AddBlockedDirection(Direction direction)
        {
            blockedFrom.Add(direction);
        }

        public void RemoveBlockedDirection(Direction direction)
        {
            blockedFrom.Remove(direction);
        }

        public bool IsBlockedFrom(Direction direction)
        {
            return blockedFrom.Count == 0 || blockedFrom.Contains(direction);
        }

        public bool BlocksMovementFrom(Direction direction)
        {
            return BlocksMovement && IsBlockedFrom(direction);
        }
    }
}
