using MonolithEngine.Engine.Source.Entities;
using MonolithEngine.Engine.Source.Global;
using MonolithEngine.Engine.Source.Level.Collision;
using MonolithEngine.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForestPlatformerExample.Source.Entities.Enemies
{
    class AIUtil
    {
        public static bool WillColliderOrFall(AbstractEnemy enemy)
        {
            if (enemy.CurrentFaceDirection == Direction.WEST)
            {
                StaticCollider southWestCollider = enemy.Scene.GridCollisionChecker.GetColliderAt(GridUtil.GetLeftBelowGrid(enemy.Transform.GridCoordinates));
                return enemy.Scene.GridCollisionChecker.HasBlockingColliderAt(enemy.Transform.GridCoordinates, Direction.WEST) || southWestCollider == null || !southWestCollider.BlocksMovementFrom(Direction.SOUTH);
            }
            else if (enemy.CurrentFaceDirection == Direction.EAST)
            {
                StaticCollider southEastCollider = enemy.Scene.GridCollisionChecker.GetColliderAt(GridUtil.GetRightBelowGrid(enemy.Transform.GridCoordinates));
                return enemy.Scene.GridCollisionChecker.HasBlockingColliderAt(enemy.Transform.GridCoordinates, Direction.EAST) || southEastCollider == null || !southEastCollider.BlocksMovementFrom(Direction.SOUTH);
            }
            throw new Exception("Wrong CurrentFaceDirection for enemy!");
        }

        public static void Patrol(bool checkCollisions, AbstractEnemy enemy)
        {
            if (enemy.Velocity.Y > 0)
            {
                return;
            }
            if (checkCollisions && WillColliderOrFall(enemy))
            {
                if (enemy.CurrentFaceDirection == Direction.WEST)
                {
                    enemy.CurrentFaceDirection = Direction.EAST;
                }
                else if (enemy.CurrentFaceDirection == Direction.EAST)
                {
                    enemy.CurrentFaceDirection = Direction.WEST;
                }
            }

            if (enemy.CurrentFaceDirection == Direction.WEST)
            {
                enemy.MoveDirection = -1;
            }
            else if (enemy.CurrentFaceDirection == Direction.EAST)
            {
                enemy.MoveDirection = 1;
            }

            enemy.VelocityX += enemy.CurrentSpeed * enemy.MoveDirection * Globals.FixedUpdateMultiplier;
        }
    }
}
