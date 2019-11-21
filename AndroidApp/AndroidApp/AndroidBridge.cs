using System;
using System.Collections.Generic;


namespace AndroidApp
{
    public static class AndroidBridge
    {

        public static readonly string TAG = typeof(AndroidBridge).Name.ToString();

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
        public static Action<string> _toast_from_back = null;
        public static void ToastItFromBack(string message)
        {
            _toast_from_back?.Invoke(message);
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
        public static Action<List<String>,Exception> WifiScanningCallback = null;     

        public static Action OnForgroundServiceStart = null;
        public static Action OnForgroundServiceStop = null;

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

        public static Func<bool> _is_service_up = null;
        public static bool isForegroundServiceUp()
        {
            return _is_service_up?.Invoke() ?? false;
        }

        public static Action _stop_all_jobs = null;
        public static Action<int> _stop_job = null;
        public static Func<
            TimeSpan?, TimeSpan?, TimeSpan?,
            Action<Action<bool>>,
            Func<bool>,
            Action,
            Func<bool>
            , int> _schedule_job = null;

        public static void stopAllJobs()
        {
            _stop_all_jobs?.Invoke();
        }

        public static void stopJob(int id)
        {
            _stop_job?.Invoke(id);
        }

        public static int scheduleJob
            (
            TimeSpan? latency, TimeSpan? maxLatency, TimeSpan? interval,
            Action<Action<bool>> onJob, Func<bool> shouldContinue, Action onJobRequirementAbort, Func<bool> shouldRetryAfterAbort
            )
        {
            return _schedule_job?.Invoke(latency, maxLatency, interval,
                onJob, shouldContinue, onJobRequirementAbort, shouldRetryAfterAbort)
                ?? -1;
        }

    }
}
