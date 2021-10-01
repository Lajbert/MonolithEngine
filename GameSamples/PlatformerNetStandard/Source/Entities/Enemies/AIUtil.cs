﻿using Microsoft.Xna.Framework;
using MonolithEngine;
using System;

namespace ForestPlatformerExample
{
    class AIUtil
    {

        private static bool changeDirectionAllowed = false;

        public static bool WillColliderOrFall(AbstractEnemy enemy)
        {
            if (enemy.CurrentFaceDirection == Direction.WEST)
            {
                StaticCollider southWestCollider = enemy.Scene.GridCollisionChecker.GetColliderAt(GridUtil.GetLeftBelowGrid(enemy));
                return enemy.Scene.GridCollisionChecker.HasBlockingColliderAt(enemy, Direction.WEST) || southWestCollider == null || !southWestCollider.BlocksMovementFrom(Direction.SOUTH);
            }
            else if (enemy.CurrentFaceDirection == Direction.EAST)
            {
                StaticCollider southEastCollider = enemy.Scene.GridCollisionChecker.GetColliderAt(GridUtil.GetRightBelowGrid(enemy));
                return enemy.Scene.GridCollisionChecker.HasBlockingColliderAt(enemy, Direction.EAST) || southEastCollider == null || !southEastCollider.BlocksMovementFrom(Direction.SOUTH);
            }
            throw new Exception("Wrong CurrentFaceDirection for enemy!");
        }

        public static void Patrol(bool checkCollisions, AbstractEnemy enemy, float waitingTime = 0f)
        {
            if (enemy.Transform.VelocityY > 0)
            {
                return;
            }
            Direction newFaceDirection = enemy.CurrentFaceDirection;
            if (checkCollisions && WillColliderOrFall(enemy))
            {
                if (waitingTime > 0 && !changeDirectionAllowed)
                {
                    Timer.SetTimer("CARROT_WAIT" + enemy.GetID(), waitingTime);
                    enemy.Transform.Velocity = Vector2.Zero;
                    changeDirectionAllowed = true;
                }
                if (enemy.CurrentFaceDirection == Direction.WEST)
                {
                    newFaceDirection = Direction.EAST;
                }
                else if (enemy.CurrentFaceDirection == Direction.EAST)
                {
                    newFaceDirection = Direction.WEST;
                }
            }

            if (newFaceDirection == Direction.WEST)
            {
                enemy.MoveDirection = -1;
            }
            else if (newFaceDirection == Direction.EAST)
            {
                enemy.MoveDirection = 1;
            }

            if (!Timer.IsSet("CARROT_WAIT" + enemy.GetID()))
            {
                enemy.CurrentFaceDirection = newFaceDirection;
                enemy.Transform.VelocityX += enemy.CurrentSpeed * enemy.MoveDirection * Globals.FixedUpdateMultiplier;
                changeDirectionAllowed = false;
            }
        }
    }
}
