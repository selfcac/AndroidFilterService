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
        public static readonly RelativeFilePath LOCK_DATE = new RelativeFilePath("polcy_lock_date.txt");
        public static readonly RelativeFilePath HTTP_POLICY = new RelativeFilePath("polcy_http_policy.json");
        public static readonly RelativeFilePath TIME_POLICY = new RelativeFilePath("polcy_time_policy.json");
        public static readonly RelativeFilePath MASTER_PASSWORD = new RelativeFilePath("master_password.txt");

        public RelativeFilePath[] EXPOSED_POLICIES = new[]
        {
            LOCK_DATE, HTTP_POLICY, TIME_POLICY
        };
    }
}

