using System;
using System.Collections.Generic;
using System.Text;

namespace AndroidApp
{
    public static class AndroidBridge
    {
        // No error handling : Critical log path
        public static Action<string, string> _log_d = null;
        public static void d(string TAG, string message)
        {
            _log_d?.Invoke(TAG, message);
        }

        // No error handling : Critical log path
        public static Action<string, string> _log_e = null;
        public static void e(string TAG, string message)
        {
            _log_e?.Invoke(TAG, message);
        }
        public static void e(string TAG, Exception exception)
        {
            _log_e?.Invoke(TAG, exception.ToString());
        }

        public static Action<string> _toast = null;
        public static void ToastIt(string message)
        {
            _toast?.Invoke(message);
        }

        public static Func<string,string, string> _readAsset = null;
        public static string ReadAssetAsString(string TAG, string assetName)
        {
            return _readAsset?.Invoke(TAG, assetName) ?? "";
        }

        // TODO: show debug info (like save path, what files exists internelly etc...)
        public static Func<string> _envDebugInfo = null;
        public static string GetEnvDebugInfo()
        {
            return _envDebugInfo?.Invoke() ?? "";
        }

        // TODO: Make Service as a long task wrapper for any Action<> 
        //          and pass task that start a server and filter stuff.
        //          all core libs shouldn't be in "App.Android" but in cross-platform "App" project


        public static Func<string, bool, string> _get_absolute_path = null;
        public static string GetAbsulotePath(string relative, bool isPublic)
        {
            return _get_absolute_path?.Invoke(relative, isPublic);
        }
    }
}
