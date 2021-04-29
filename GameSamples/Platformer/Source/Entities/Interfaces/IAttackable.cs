using MonolithEngine.Engine.Source.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForestPlatformerExample.Source.Entities.Interfaces
{
    interface IAttackable
    {
        public void Hit(Direction impactDirection);
    }
}
