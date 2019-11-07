using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AndroidApp
{
    class Logger
    {
        const char debug_char = 'D';
        const char error_char = 'E';
        const string logFormat = "log_{0}.txt";

        public const string TAG = "logger_app";

        private static string lineFormat(string myTag, char Level, string message)
        {
            return string.Format("[{0}] {1}/{2} {3}", DateTime.Now, Level, myTag, message);
        }

        private static void logToPublicFile(string filename, string TAG, char level, string message, bool writeToFile )
        {
            try
            {
                if (writeToFile)
                {
                    File.WriteAllLines(
                        AndroidBridge.GetAbsulotePath(filename, true),
                        new[] { lineFormat(TAG, level, message) }
                        );
                }

                switch (level)
                {
                    case debug_char:
                        AndroidBridge.d(TAG, message);
                        break;

                    case error_char:
                        AndroidBridge.e(TAG, message);
                        break;
                }
            }
            catch (Exception ex)
            {
                AndroidBridge.e(Logger.TAG, ex);
            }
        }

        public static void d(string TAG, string message,bool writeToFile = true)
        {
            char myChar = debug_char;
            logToPublicFile(string.Format(logFormat, myChar),TAG, myChar, message, writeToFile);
        }

        public static void e(string TAG, string message, bool writeToFile = true)
        {
            char myChar = error_char;
            logToPublicFile(string.Format(logFormat, myChar), TAG, myChar, message, writeToFile);
        }

        public static void e(string TAG, Exception ex)
        {
            e(TAG, ex.ToString());
        }


    }
}
