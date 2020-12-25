using System;
using System.Collections.Generic;
using System.Text;

namespace _2DGameEngine.Util
{
    class Logger
    {
        public static void Log(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }
    }
}
