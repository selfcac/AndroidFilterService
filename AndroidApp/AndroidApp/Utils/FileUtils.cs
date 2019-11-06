using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AndroidApp.Utils
{
    class FileUtils
    {
        public const string TAG = "FileUtils";
        public static void AppendLinePublic(string relativePath, string line)
        {
            try
            {
                File.AppendAllLines(AndroidBridge.GetAbsulotePath(relativePath, true), new[] { line });
            }
            catch (Exception ex)
            {
                AndroidBridge.e(TAG,ex);
            }
        }

        public static void AppendLineInternal(string relativePath, string line)
        {
            try
            {
                File.AppendAllLines(AndroidBridge.GetAbsulotePath(relativePath, false), new[] { line });
            }
            catch (Exception ex)
            {
                AndroidBridge.e(TAG, ex);
            }
        }
    }
}
