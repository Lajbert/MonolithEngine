using System;

namespace MonolithEngine
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

        public static void Info(object toLog)
        {
            System.Diagnostics.Debug.WriteLine(DateTime.Now + " [INFO]: " + toLog.ToString());
        }

        public static void Debug(object toLog)
        {
            System.Diagnostics.Debug.WriteLine(DateTime.Now + " [DEBUG]: " + toLog.ToString());
        }

        public static void Warn(object toLog)
        {
            System.Diagnostics.Debug.WriteLine(DateTime.Now + " [WARNING]: " + toLog.ToString());
        }

        public static void Error(object toLog)
        {
            System.Diagnostics.Debug.WriteLine(DateTime.Now + " [ERROR]: " + toLog.ToString());
        }
    }
}
