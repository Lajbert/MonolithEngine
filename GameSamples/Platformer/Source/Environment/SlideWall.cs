using Microsoft.Xna.Framework;
using MonolithEngine;
using System;

namespace ForestPlatformerExample
{
    class SlideWall : Entity
    {
        public SlideWall(AbstractScene scene, Vector2 position, int width, int height) : base(scene.LayerManager.EntityLayer, null, position)
        {
            if (width == 0 || height == 0)
            {
                throw new Exception("Invalid slide wall dimensions!");
            }
            AddComponent(new BoxCollisionComponent(this, width, height));    
            AddTag("Environment");
            Active = false;
#if DEBUG
            //(GetCollisionComponent() as BoxCollisionComponent).DEBUG_DISPLAY_COLLISION = true;
            //Visible = true;
#else
            Visible = false;
#endif
        }
    }
}
