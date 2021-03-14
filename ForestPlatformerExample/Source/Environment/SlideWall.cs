using GameEngine2D.Engine.Source.Physics.Collision;
using GameEngine2D.Entities;
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
