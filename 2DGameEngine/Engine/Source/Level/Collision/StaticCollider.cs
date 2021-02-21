using GameEngine2D.Engine.Source.Entities;
using GameEngine2D.Engine.Source.Physics.Collision;
using GameEngine2D.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.Engine.Source.Level.Collision
{
    public class StaticCollider : GameObject
    {

        public StaticCollider(Vector2 gridPosition) : base(null)
        {
            Transform = new StaticTransform(this);
            Transform.GridCoordinates = gridPosition;
            GridCollisionChecker.Instance.Add(this);
        }

        private HashSet<Direction> blockedFrom = new HashSet<Direction>();

        public bool BlocksMovement = true;

        public override void Destroy()
        {
            GridCollisionChecker.Instance.Remove(this);
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
