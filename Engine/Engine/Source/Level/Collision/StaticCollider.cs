using MonolithEngine.Engine.Source.Entities;
using MonolithEngine.Engine.Source.Physics.Collision;
using MonolithEngine.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using MonolithEngine.Engine.Source.Scene;

namespace MonolithEngine.Engine.Source.Level.Collision
{
    public class StaticCollider : GameObject
    {

        private AbstractScene scene;

        public StaticCollider(AbstractScene scene, Vector2 gridPosition) : base(null)
        {
            Transform = new StaticTransform(this)
            {
                GridCoordinates = gridPosition
            };
            this.scene = scene;
            scene.GridCollisionChecker.Add(this);
        }

        private HashSet<Direction> blockedFrom = new HashSet<Direction>();

        public bool BlocksMovement = true;

        public override void Destroy()
        {
            scene.GridCollisionChecker.Remove(this);
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

        public override bool IsAlive()
        {
            throw new NotImplementedException();
        }
    }
}
