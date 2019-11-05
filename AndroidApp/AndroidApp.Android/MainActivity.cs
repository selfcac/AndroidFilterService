using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V4.App;

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

            base.OnCreate(savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }

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
                ActivityCompat.RequestPermissions(this, permissions , 0);
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

        public void setupAndroidBridge()
        {
            AndroidBridge._log_d_from_main = new Action<string, string>((tag, msg) =>
            {
                FileHelpers.AppendLinePublic(this, "activity_log.txt", AndroidUtils.logFormat(tag, 'D', msg));
            });

            AndroidBridge._log_e_from_main = new Action<string, string>((tag, msg) =>
            {
                FileHelpers.AppendLinePublic(this, "activity_err.txt", AndroidUtils.logFormat(tag, 'D', msg));
            });
        }
    } 
}