using System;
using System.Collections.Generic;
using System.Text;

namespace _2DGameEngine.Global
{
    class Constants
    {
        public static readonly float CHARACTER_SPEED = 3f;
        public static readonly int GRID = 64;
        public static readonly float FRICTION = 0.2f;
        public static readonly float BUMB_FRICTION = 0.5f;
        public static readonly bool GRAVITY_ON = true;
        public static readonly float GRAVITY_FORCE = 8f;
        public static readonly float JUMP_FORCE = 10f;
        public static readonly float JUMP_T_MULTIPLIER = 2f;

        public static readonly float TIME_OFFSET = 10;
        public static readonly int RES_W = 1920;
        public static readonly int RES_H = 1080;
        public static readonly int FPS = 60;
        public static readonly int PIVOT_DIAM = 10;
    }
}
