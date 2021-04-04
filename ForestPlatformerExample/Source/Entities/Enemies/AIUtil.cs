using MonolithEngine.Engine.Source.Entities;
using MonolithEngine.Engine.Source.Global;
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
                return enemy.Scene.GridCollisionChecker.HasBlockingColliderAt(enemy.Transform.GridCoordinates, Direction.WEST) || !enemy.Scene.GridCollisionChecker.HasBlockingColliderAt(enemy.Transform.GridCoordinates, Direction.SOUTHWEST);
            }
            else if (enemy.CurrentFaceDirection == Direction.EAST)
            {
                return enemy.Scene.GridCollisionChecker.HasBlockingColliderAt(enemy.Transform.GridCoordinates, Direction.EAST) || !enemy.Scene.GridCollisionChecker.HasBlockingColliderAt(enemy.Transform.GridCoordinates, Direction.SOUTHEAST);
            }
            throw new Exception("Wrong CurrentFaceDirection for enemy!");
        }

        public static void Patrol(bool checkCollisions, AbstractEnemy enemy)
        {
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
