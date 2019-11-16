using System;
using System.Collections.Generic;
using System.Text;

namespace AndroidApp.FilterUtils
{
    class Filenames
    {
        public const string LOCK_DATE = "polcy_lock_date.txt";
        public const string HTTP_POLICY = "polcy_http_policy.json";
        public const string TIME_POLICY = "polcy_time_policy.json";
        public const string MASTER_PASSWORD = "master_password.txt";

        public string[] EXPOSED_POLICIES = new[]
        {
            LOCK_DATE, HTTP_POLICY, TIME_POLICY
        };
    }
}

