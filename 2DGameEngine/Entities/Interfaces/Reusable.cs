﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace _2DGameEngine.Entities.Interfaces
{
    interface Reusable
    {
        public void Reset(Vector2 position = new Vector2());
    }
}
