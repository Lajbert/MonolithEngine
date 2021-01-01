using GameEngine2D.Global;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.src.Util
{
    class TimeUtil
    {
        public static float GetElapsedTime(GameTime gameTime)
        {
            return (float)gameTime.ElapsedGameTime.TotalSeconds * Config.TIME_OFFSET;
        }
    }
}
