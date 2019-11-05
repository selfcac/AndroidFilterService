using System;
using System.Collections.Generic;
using System.Text;

namespace AndroidApp
{
    public static class AndroidBridge
    {
        // No error handling : Critical log path
        public static Action<string, string> _log_d_from_service = null;
        public static void serviceLog(string TAG, string message)
        {
            _log_d_from_service?.Invoke(TAG, message);
        }

        // No error handling : Critical log path
        public static Action<string, string> _log_e_from_service = null;
        public static void serviceError(string TAG, string message)
        {
            _log_e_from_service?.Invoke(TAG, message);
        }
        public static void serviceError(string TAG, Exception exception)
        {
            _log_e_from_service?.Invoke(TAG, exception.ToString());
        }

        // No error handling : Critical log path
        public static Action<string, string> _log_d_from_main = null;
        public static void activityLog(string TAG, string message)
        {
            _log_d_from_main?.Invoke(TAG, message);
        }

        // No error handling : Critical log path
        public static Action<string, string> _log_e_from_main = null;
        public static void activityError(string TAG, string message)
        {
            _log_e_from_main?.Invoke(TAG, message);
        }
        public static void activityError(string TAG, Exception exception)
        {
            _log_e_from_main?.Invoke(TAG, exception.ToString());
        }


    }
}
