using GameEngine2D.Engine.Source.Physics.Trigger;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.Engine.Source.Entities.Interfaces
{
    public interface IHasTrigger
    {
        public ITrigger GetTrigger();
    }
}
