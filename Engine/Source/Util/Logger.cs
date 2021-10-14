using System;
using System.IO;

namespace MonolithEngine
{
    public class Logger
    {

        private static string ERROR = "ERROR";
        private static string INFO = "INFO";
        private static string DEBUG = "DEBUG";
        private static string WARNING = "WARNING";

        //private static StreamWriter file;

        static Logger()
        {
            //file = new StreamWriter("Debug.log", append: true);
        }

        public static bool LogToFile = false;

        public static void Info(string message)
        {
            Log(INFO, message);
        }

        public static void Debug(string message)
        {
            Log(DEBUG, message);
        }

        public static void Warn(string message)
        {
            Log(WARNING, message);
        }

        public static void Error(string message)
        {
            Log(ERROR, message);
        }

        public static void Info(object toLog)
        {
            Log(INFO, toLog.ToString());
        }

        public static void Debug(object toLog)
        {
            Log(DEBUG, toLog.ToString());
        }

        public static void Warn(object toLog)
        {
            Log(WARNING, toLog.ToString());
        }

        public static void Error(object toLog)
        {
            Log(ERROR, toLog.ToString());
        }

        private static void Log(string level, string message)
        {
            string logMessage = DateTime.Now + " [" + level + "]: " + message;
            if (level == DEBUG)
            {
                System.Diagnostics.Debug.WriteLine(logMessage);
            }
            else
            {
                System.Diagnostics.Trace.WriteLine(logMessage);
            }

            /*if (LogToFile)
            {
                file.WriteLine(logMessage);
                file.Flush();
            }*/
        }
    }
}
