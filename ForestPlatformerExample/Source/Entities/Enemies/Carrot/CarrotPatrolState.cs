using ForestPlatformerExample.Source.Enemies;
using GameEngine2D.Engine.AI;
using GameEngine2D.Engine.Source.Entities;
using GameEngine2D.Engine.Source.Physics.Collision;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForestPlatformerExample.Source.Entities.Enemies.CarrotAI
{
    class CarrotPatrolState : AIState<Carrot>
    {

        private int direction = 1;

        protected bool checkCollisions;

        public CarrotPatrolState(Carrot carrot) : base(carrot)
        {
        }

        public override void Begin()
        {
            checkCollisions = true;
            base.Begin();
        }

        public override void Update(GameTime gameTime)
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
                direction *= -1;
            }

            controlledEntity.VelocityX += controlledEntity.CurrentSpeed * direction * (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            base.Update(gameTime);
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
