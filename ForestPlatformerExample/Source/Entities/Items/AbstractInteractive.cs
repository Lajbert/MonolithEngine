using MonolithEngine;
using MonolithEngine.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForestPlatformerExample.Source.Entities.Items
{
    class AbstractInteractive : PhysicalEntity
    {
        public AbstractInteractive(Vector2 position) : base(LayerManager.Instance.EntityLayer, null, position) {
            AddTag("Interactive");
        }
    }
}
