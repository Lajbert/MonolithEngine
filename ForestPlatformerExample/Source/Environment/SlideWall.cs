using MonolithEngine.Engine.Source.Physics.Collision;
using MonolithEngine.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForestPlatformerExample.Source.Environment
{
    class SlideWall : Entity
    {
        public SlideWall(Vector2 position, int width, int height) : base(LayerManager.Instance.EntityLayer, null, position)
        {
            if (width == 0 || height == 0)
            {
                throw new Exception("Invalid slide wall dimensions!");
            }
            AddComponent(new BoxCollisionComponent(this, width, height));
            (GetCollisionComponent() as BoxCollisionComponent).DEBUG_DISPLAY_COLLISION = true;
            AddTag("Environment");
            Active = false;
#if DEBUG
            Visible = true;
#else
            Visible = false;
#endif
        }
    }
}
