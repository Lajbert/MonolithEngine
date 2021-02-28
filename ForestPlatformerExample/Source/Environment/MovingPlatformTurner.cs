using GameEngine2D.Engine.Source.Entities;
using GameEngine2D.Engine.Source.Physics.Collision;
using GameEngine2D.Engine.Source.Physics.Trigger;
using GameEngine2D.Entities;
using GameEngine2D.Global;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForestPlatformerExample.Source.Environment
{
    class MovingPlatformTurner : Entity
    {
        public Direction TurnDirection;

        public MovingPlatformTurner(Vector2 position, Direction turnDirection) : base(LayerManager.Instance.EntityLayer, null, position)
        {
            TurnDirection = turnDirection;
            //Active = false;
            //Visible = false;
            AddComponent(new BoxCollisionComponent(this, Config.GRID, Config.GRID));
            //(GetCollisionComponent() as AbstractCollisionComponent).DEBUG_DISPLAY_COLLISION = true;
            AddTag("PlatformTurner");
        }
    }
}
