using System;
using System.Collections.Generic;
using System.Text;

namespace _2DGameEngine.Entities.Interfaces
{
    interface IGravityApplicable
    {
        public bool HasGravity();

        public float GetGravityConstant();
    }
}
