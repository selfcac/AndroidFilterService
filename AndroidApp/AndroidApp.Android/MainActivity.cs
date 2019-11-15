using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V4.App;
using Android.Content;
using Android.Util;

namespace AndroidApp.Droid
{
    [Activity(Label = "selfcac", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, ActivityCompat.IOnRequestPermissionsResultCallback
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            RunUnderPermission(new[] { PermissionConstants.WRITE_EXTERNAL_STORAGE }, null, ()=>
            {
                AndroidUtils.ToastIt(this, "Permission Denied, Exiting.");
                Finish();
            });

            setupAndroidBridge();

            base.OnCreate(savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }

        #region CriticalPermissionRequestAtInit

        Action[] _requestedActionUnderPermissions = null;
        public void RunUnderPermission(string[] permissions, Action callback, Action onFail)
        {
            // No error handling : Critical log path

            bool allAllowed = true;
            foreach (string p in permissions)
            {
                if (CheckSelfPermission(p) == Permission.Denied)
                {
                    allAllowed = false;
                    break;
                }
            }

            if (allAllowed)
            {
                callback?.Invoke();
            }
            else
            {
                Action[] callbackArray = new Action[] { callback, onFail };
                _requestedActionUnderPermissions = callbackArray;
                ActivityCompat.RequestPermissions(this, permissions, 0);
            }
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            // No error handling : Critical log path

            if (_requestedActionUnderPermissions != null)
            {
                Action[] actions = _requestedActionUnderPermissions;
                _requestedActionUnderPermissions = null;

                bool allAllowed = true;
                foreach (Permission p in grantResults)
                {
                    if (p == Permission.Denied)
                    {
                        allAllowed = false;
                        break;
                    }
                }

                if (allAllowed)
                {
                    actions[0]?.Invoke(); // on sucess callback
                }
                else
                {
                    actions[1]?.Invoke(); // onFail
                }
            }
        }

        #endregion

        public void setupAndroidBridge()
        {
            Context global_ctx = Application.Context;

            AndroidBridge._log_d = new Action<string, string>((tag, msg) =>
            {
                AndroidLevelLogger.d(tag, msg);
            });

            AndroidBridge._log_e = new Action<string, string>((tag, msg) =>
            {
                AndroidLevelLogger.e(tag, msg);
            });

            AndroidBridge._toast = new Action<string>(( msg) =>
            {
                try
                {
                    AndroidUtils.ToastIt(global_ctx, msg);
                }
                catch (Exception ex)
                {
                    Log.Error(AndroidBridge.TAG, ex.ToString());
                }
            });

            AndroidBridge._toast_from_back = new Action<string>((msg) =>
            {
                try
                {
                    AndroidUtils.ToastItFromBack(global_ctx, msg);
                }
                catch (Exception ex)
                {
                    Log.Error(AndroidBridge.TAG, ex.ToString());
                }
            });

            AndroidBridge._readAsset = new Func<string, string, string>((tag, asset_name) =>
            {
                string result = "";
                try
                {
                    result = FileHelpers.ReadAssetAsString(global_ctx, tag, asset_name);
                }
                    catch (Exception ex)
                {
                    Log.Error(AndroidBridge.TAG, ex.ToString());
                }
                return result;
            });

            AndroidBridge._get_absolute_path = new Func<string, bool, string>((relative, isPulic) =>
            {
                string result = "";
                try
                {
                    if( isPulic )
                    {
                        result = FileHelpers.getPublicAppFilePath(global_ctx, relative);
                    }
                    else
                    {
                        result = FileHelpers.getInternalAppFilePath(global_ctx, relative);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(AndroidBridge.TAG, ex.ToString());
                }
                return result;
            });

            WifiScan.InitWifiScan(global_ctx);

           

            AndroidBridge._start_wifi_scan = new Action(() =>
            {
                WifiScan.RequestScan(global_ctx);
            });

            AndroidBridge._start_service = new Action(() =>
            {
                MyForegroundService.StartForegroundServiceCompat<MyForegroundService>(global_ctx, MyForegroundService.ACTION_START_SERVICE);
            });

            AndroidBridge._stop_service = new Action(() =>
            {
                MyForegroundService.StartForegroundServiceCompat<MyForegroundService>(global_ctx, MyForegroundService.ACTION_STOP_SERVICE);
            });


            AndroidBridge._stop_all_jobs = () =>
            {
                PeriodicTask.cancelAllJobs(global_ctx);
            };

            AndroidBridge._stop_job = (int id) =>
            {
                PeriodicTask.cancelJobById(global_ctx, id);
            };

            AndroidBridge._schedule_job = new Func<
                TimeSpan?, TimeSpan?, TimeSpan?,Action<Action<bool>>, Func<bool>, Action, Func<bool>, int>(
            (latency, maxLatency, interval, _onJob, _shouldContinue, _onJobRequirementAbort, _shouldRetryAfterAbort) =>
            {
                PeriodicTask.JobCallbacks job = new PeriodicTask.JobCallbacks()
                {
                    onJob = _onJob,
                    shouldContinue = _shouldContinue,
                    onJobRequirementAbort = _onJobRequirementAbort,
                    shouldRetryAfterAbort = _shouldRetryAfterAbort
                };
                if (PeriodicTask.scheduleJob(global_ctx, job,latency, maxLatency, interval))
                {
                    return job.JobUniqueID;
                }
                return -1;
            }
            );
        }
    } 
}