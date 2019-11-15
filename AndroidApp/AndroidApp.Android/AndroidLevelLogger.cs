using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Android.Util;
using System.IO;

namespace AndroidApp.Droid
{
    class AndroidLevelLogger
    {
        #region Constants


        static readonly string TAG = typeof(AndroidLevelLogger).Name.ToString();
        delegate int LogFunction(string tag, string messgae);
        enum LogLevels
        {
            DEBUG, INFO, WARN, ERROR
        }

        static Dictionary<LogLevels, char> _log_level_toChar = new Dictionary<LogLevels, char>()
        {
            {LogLevels.DEBUG, 'D' },
            {LogLevels.INFO, 'I' },
            {LogLevels.WARN, 'W' },
            {LogLevels.ERROR, 'E' },
        };

        static Dictionary<char, LogFunction> _log_char_to_f = new Dictionary<char, LogFunction>() {
            {'D', Log.Debug },
            {'I', Log.Info },
            {'W', Log.Warn },
            {'E', Log.Error },
        };
        const string logFormat = "log_{0}.txt"; 

        #endregion



        public static void AppendLinePublic(string relativePath, string line)
        {
            try
            {
                File.AppendAllLines(AndroidBridge.GetAbsulotePath(relativePath, isPublic: true), new[] { line });
            }
            catch (Exception ex)
            {
                AndroidBridge.e(TAG, ex);
            }
        }       

        private static string fileDebugLineFormat(string myTag, char Level, string message)
        {
            return string.Format("[{0}] {1}/{2} {3}", DateTime.Now, Level, myTag, message);
        }

        private static void AndroidLogFlow(string filename, string f_TAG, char level, string message, bool writeToFile)
        {
            try
            {
                // First log to platform (no permission needed) inner log
                if (_log_char_to_f.ContainsKey(level))
                {
                    _log_char_to_f[level]?.Invoke(f_TAG, message);
                }
                else
                {
                    Log.Debug(f_TAG, string.Format("[{0}] {1}", level, message));
                }

                // Second log to file, might fail based on permissions
                if (writeToFile)
                {
                    AppendLinePublic(filename, fileDebugLineFormat(f_TAG, level, message));
                }                
            }
            catch (Exception ex)
            {
                Log.Error(TAG, ex.ToString());
            }
        }

        public static void d(string TAG, string message, bool writeToFile = true)
        {
            char myChar = _log_level_toChar[LogLevels.DEBUG];
            AndroidLogFlow(string.Format(logFormat, myChar), TAG, myChar, message, writeToFile);
        }

        public static void i(string TAG, string message, bool writeToFile = true)
        {
            char myChar = _log_level_toChar[LogLevels.INFO];
            AndroidLogFlow(string.Format(logFormat, myChar), TAG, myChar, message, writeToFile);
        }

        public static void w(string TAG, string message, bool writeToFile = true)
        {
            char myChar = _log_level_toChar[LogLevels.WARN];
            AndroidLogFlow(string.Format(logFormat, myChar), TAG, myChar, message, writeToFile);
        }

        public static void e(string TAG, string message, bool writeToFile = true)
        {
            char myChar = _log_level_toChar[LogLevels.ERROR];
            AndroidLogFlow(string.Format(logFormat, myChar), TAG, myChar, message, writeToFile);
        }

        public static void e(string TAG, Exception ex)
        {
            e(TAG, ex.ToString());
        }
    }
}