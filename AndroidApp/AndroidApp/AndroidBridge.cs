using System;
using System.Collections.Generic;
using System.Text;

namespace AndroidApp
{
    public static class AndroidBridge
    {
        // No error handling : Critical log path
        public static Action<string, string> _log_d = null;
        public static void activityLog(string TAG, string message)
        {
            _log_d?.Invoke(TAG, message);
        }

        // No error handling : Critical log path
        public static Action<string, string> _log_e = null;
        public static void activityError(string TAG, string message)
        {
            _log_e?.Invoke(TAG, message);
        }
        public static void activityError(string TAG, Exception exception)
        {
            _log_e?.Invoke(TAG, exception.ToString());
        }

        public static Action<string> _toast = null;
        public static void activityToast(string message)
        {
            _toast?.Invoke(message);
        }
    }
}
