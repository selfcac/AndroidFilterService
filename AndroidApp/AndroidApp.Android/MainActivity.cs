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

            base.OnCreate(savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }

        JavaDictionary<int, Action> _requestedActionUnderPermissions = new JavaDictionary<int, Action>();
        public void RunUnderPermission(Action callback, string[] permissions)
        {
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
                int reqCode = callback.GetHashCode();
                _requestedActionUnderPermissions.Add(reqCode, callback);
                ActivityCompat.RequestPermissions(this, permissions , reqCode);
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            if (_requestedActionUnderPermissions.ContainsKey(requestCode))
            {
                Action callback = _requestedActionUnderPermissions[requestCode];
                _requestedActionUnderPermissions.Remove(requestCode);

                bool allAllowed = true;
                for (Permission p in grantResults)
                {
                    if (p == Permission.Denied)
                    {
                        allAllowed = false;
                        break;
                    }
                }

                if (allAllowed)
                {
                    callback?.Invoke();
                }
            }
        }


    } 
}