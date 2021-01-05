using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.Util
{
    public class Logger
    {
        public static void Log(string message)
        {
            System.Diagnostics.Debug.WriteLine(DateTime.Now + " [DEBUG]: " + message);
        }
    }
}
