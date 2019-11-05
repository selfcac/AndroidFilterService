using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;

namespace AndroidApp.Droid
{
    class AndroidUtils
    {
        public static void ToastIt(Context ctx, string message, bool isLong = true)
        {
            Toast.MakeText(ctx, message, isLong ? ToastLength.Long : ToastLength.Short).Show();
        }

        public static string logFormat(string myTag, char Level, string message)
        {
            return string.Format("[{0}] {1}/{2} {3}", DateTime.Now, Level, myTag, message);
        }
    }
}