using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace MonolithEngine
{
    /// <summary>
    /// A class to represent static (environmental) collisions on a level.
    /// These colliders and not changing their position throughout the game and
    /// therefore part of a 2D collision grid.
    /// </summary>
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

        public override void AddChild(IGameObject gameObject)
        {
            throw new NotImplementedException();
        }

        public override void RemoveChild(IGameObject gameObject)
        {
            throw new NotImplementedException();
        }
    }
}
