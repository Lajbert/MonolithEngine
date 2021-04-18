using Microsoft.Xna.Framework;
using MonolithEngine.Engine.Source.Entities;
using MonolithEngine.Engine.Source.Scene;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForestPlatformerExample.Source.Entities.Enemies.IceCream
{
    class IceCream : AbstractEnemy
    {

        public IceCream(AbstractScene scene, Vector2 position) : base (scene, position)
        {

        }

        public override void Hit(Direction impactDireciton)
        {
            throw new NotImplementedException();
        }
    }
}
