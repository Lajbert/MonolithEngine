using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.Util
{
    public class Logger
    {
        public static void Info(string message)
        {
            System.Diagnostics.Debug.WriteLine(DateTime.Now + " [INFO]: " + message);
        }

        public static void Debug(string message)
        {
            System.Diagnostics.Debug.WriteLine(DateTime.Now + " [DEBUG]: " + message);
        }

        public static void Warn(string message)
        {
            System.Diagnostics.Debug.WriteLine(DateTime.Now + " [WARNING]: " + message);
        }

        public static void Error(string message)
        {
            System.Diagnostics.Debug.WriteLine(DateTime.Now + " [ERROR]: " + message);
        }
    }
}
