using MonolithEngine.Engine.Source.Components;
using MonolithEngine.Engine.Source.Entities.Abstract;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonolithEngine.Engine.Source.Physics.Trigger
{
    public interface ITrigger : IComponent
    {
        public bool IsInsideTrigger(IGameObject otherObject);

        public string GetTag();
    }
}
