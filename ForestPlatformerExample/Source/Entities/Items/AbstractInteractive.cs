using MonolithEngine;
using MonolithEngine.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using MonolithEngine.Engine.Source.Scene;

namespace ForestPlatformerExample.Source.Entities.Items
{
    class AbstractInteractive : PhysicalEntity
    {
        public AbstractInteractive(AbstractScene scene, Vector2 position) : base(scene.LayerManager.EntityLayer, null, position) {
            AddTag("Interactive");
        }
    }
}
