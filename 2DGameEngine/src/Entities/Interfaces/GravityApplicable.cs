using System;
using System.Collections.Generic;
using System.Text;

namespace _2DGameEngine.Entities.Interfaces
{
    interface GravityApplicable
    {
        public bool HasGravity();

        public float GetGravityConstant();
    }
}
