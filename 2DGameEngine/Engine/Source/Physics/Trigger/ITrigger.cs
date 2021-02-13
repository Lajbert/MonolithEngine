using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.Engine.Source.Physics.Trigger
{
    public interface ITrigger
    {
        public bool IsInsideTrigger(Vector2 point);

        public string GetTag();
    }
}
