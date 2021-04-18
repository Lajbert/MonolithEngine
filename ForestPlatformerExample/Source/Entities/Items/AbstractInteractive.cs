using MonolithEngine;
using MonolithEngine.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using MonolithEngine.Engine.Source.Scene;
using ForestPlatformerExample.Source.Entities.Enemies;

namespace ForestPlatformerExample.Source.Entities.Items
{
    class AbstractInteractive : AbstractDestroyable
    {
        public AbstractInteractive(AbstractScene scene, Vector2 position) : base(scene, position) {
            AddTag("Interactive");
        }
    }
}
