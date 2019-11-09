using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Android.Util;

namespace AndroidApp.Droid
{
    public class FileHelpers
    {
        public static string getPublicAppFilePath(Context ctx, string relativePath)
        {
            return Path.Combine(ctx.GetExternalFilesDir(null).AbsolutePath, relativePath);
        }

        public static string getInternalAppFilePath(Context ctx, string relativePath)
        {
            // No error handling : Critical log path
            return Path.Combine(ctx.FilesDir.AbsolutePath, relativePath);
        }

        public static string ReadAssetAsString(Context ctx, string tag, string asset_name)
        {
            string result = "";
            try
            {
                using (Stream s = ctx.Assets.Open(asset_name))
                {
                    StreamReader reader = new StreamReader(s);
                    result = reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                AndroidBridge.e(tag, ex);
            }
            return result;
        }


    }
}