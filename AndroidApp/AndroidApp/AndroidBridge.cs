using System;
using System.Collections.Generic;
using System.Text;

namespace AndroidApp
{
    public static class AndroidBridge
    {
        public const string TAG = "AndroidBridge";

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

        public static Func<string, bool, string> _get_absolute_path = null;
        public static string GetAbsulotePath(string relative, bool isPublic)
        {
            return _get_absolute_path?.Invoke(relative, isPublic);
        }

        public static Action _start_wifi_scan = null;
        public static void StartWifiScanning()
        {
            _start_wifi_scan?.Invoke();
        }
        public static Action<List<String>,bool> WifiScanningCallbackSucess = null;

        public static Action OnForgroundServiceStart = null;
        public static Action OnForgroundServiceStop = null;

        public static Func<string> OnServiceInfoRequest = null;

        public static Action _start_service = null;
        public static void StartForgroundService()
        {
            _start_service?.Invoke();
        }

        public static Action _stop_service = null;
        public static void StopForgroundService()
        {
            _stop_service?.Invoke();
        }
    }
}
