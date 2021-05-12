using Microsoft.Xna.Framework;
using System;

namespace MonolithEngine
{
    public class Globals
    {
        public static float ElapsedTime = 0f;
        public static float FixedUpdateMultiplier = 0f;
        public static GameTime GameTime;
        public static float NextTickTime = 0;
        public static float FixedUpdateAlpha;
        public static TimeSpan FixedUpdateRate;
    }
}
