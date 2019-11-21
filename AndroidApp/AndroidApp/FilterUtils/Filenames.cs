using System;
using System.Collections.Generic;
using System.Text;

namespace AndroidApp.FilterUtils
{
    class RelativeFilePath
    {
        string _relative = "";
        public RelativeFilePath(string relative)
        {
            _relative = relative;
        }

        public string getAppPublic()
        {
            return AndroidBridge.GetAbsulotePath(_relative, true);
        }

        public string getAppPrivate()
        {
            return AndroidBridge.GetAbsulotePath(_relative, false);
        }

        public string getRelativeAbsolute()
        {
            return _relative;
        }
    }

    class Filenames
    {
        // Private:
        public static readonly RelativeFilePath LOCK_DATE = new RelativeFilePath("polcy_lock_date.txt");
        public static readonly RelativeFilePath MASTER_PASSWORD = new RelativeFilePath("master_password.txt");

        // May Exposed:
        public static readonly RelativeFilePath HTTP_POLICY = new RelativeFilePath("http_policy.json");
        public static readonly RelativeFilePath TIME_POLICY = new RelativeFilePath("time_policy.json");
        public static readonly RelativeFilePath WIFI_POLICY = new RelativeFilePath("wifi_policy.json");

        // Public:
        public static readonly RelativeFilePath BLOCK_LOG = new RelativeFilePath("block_log.txt");

        public RelativeFilePath[] EXPOSED_POLICIES = new[]
        {
            WIFI_POLICY, HTTP_POLICY, TIME_POLICY
        };
    }
}

