using GameEngine2D.Engine.Source.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForestPlatformerExample.Source.Entities.Interfaces
{
    interface IPunchable
    {
        public void Punch(Direction impactDireciton);
    }
}
