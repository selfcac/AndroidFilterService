using System;
using System.Collections.Generic;
using System.Text;

namespace AndroidApp
{
    class Logger
    {
        private static string lineFormat(string myTag, char Level, string message)
        {
            return string.Format("[{0}] {1}/{2} {3}", DateTime.Now, Level, myTag, message);
        }
    }
}
