using Microsoft.Xna.Framework;
using MonolithEngine;

namespace ForestPlatformerExample
{
    class MovingPlatformTurner : Entity
    {
        public Direction TurnDirection;

        public MovingPlatformTurner(AbstractScene scene, Vector2 position, Direction turnDirection) : base(scene.LayerManager.EntityLayer, null, position)
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
