using ForestPlatformerExample.Source.Enemies;
using GameEngine2D.Engine.AI;
using GameEngine2D.Engine.Source.Entities;
using GameEngine2D.Engine.Source.Global;
using GameEngine2D.Engine.Source.Physics.Collision;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForestPlatformerExample.Source.Entities.Enemies.CarrotAI
{
    class CarrotPatrolState : AIState<Carrot>
    {

        private int moveDirection = 1;

        protected bool checkCollisions;

        public CarrotPatrolState(Carrot carrot) : base(carrot)
        {
        }

        public override void Begin()
        {
            checkCollisions = true;
            controlledEntity.CurrentSpeed = controlledEntity.DefaultSpeed;
            base.Begin();
        }

        public override void Update()
        {

            if (checkCollisions && WillCollideOrFall())
            {
                if (controlledEntity.CurrentFaceDirection == Direction.WEST)
                {
                    controlledEntity.CurrentFaceDirection = Direction.EAST;
                }
                else if (controlledEntity.CurrentFaceDirection == Direction.EAST)
                {
                    controlledEntity.CurrentFaceDirection = Direction.WEST;
                }
            }

            if (controlledEntity.CurrentFaceDirection == Direction.WEST)
            {
                moveDirection = -1;
            } else if (controlledEntity.CurrentFaceDirection == Direction.EAST)
            {
                moveDirection = 1;
            }

            controlledEntity.VelocityX += controlledEntity.CurrentSpeed * moveDirection * Globals.ElapsedTime;

            base.Update();
        }

        private bool WillCollideOrFall()
        {
            if (controlledEntity.CurrentFaceDirection == Direction.WEST)
            {
                return GridCollisionChecker.Instance.HasBlockingColliderAt(controlledEntity.Transform.GridCoordinates, Direction.WEST) || !GridCollisionChecker.Instance.HasBlockingColliderAt(controlledEntity.Transform.GridCoordinates, GameEngine2D.Engine.Source.Entities.Direction.SOUTHWEST);
            }
            else if (controlledEntity.CurrentFaceDirection == Direction.EAST)
            {
                return GridCollisionChecker.Instance.HasBlockingColliderAt(controlledEntity.Transform.GridCoordinates, Direction.EAST) || !GridCollisionChecker.Instance.HasBlockingColliderAt(controlledEntity.Transform.GridCoordinates, GameEngine2D.Engine.Source.Entities.Direction.SOUTHEAST);
            }
            throw new Exception("Wrong CurrentFaceDirection for carrot!");
        }

    }
}
