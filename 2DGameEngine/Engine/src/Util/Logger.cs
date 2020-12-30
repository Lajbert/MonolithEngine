using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.Util
{
    class Logger
    {
        public static void Log(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }
    }
}
