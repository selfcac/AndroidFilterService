using System;
using System.Collections.Generic;
using System.Text;

namespace AndroidApp.FilterUtils
{
    class FilteringServiceFlow
    {

        static readonly string TAG = typeof(FilteringServiceFlow).Name.ToString();

        public bool isFiltering = true; // Should we filter with http\time?

        public void StartFlow()
        {
            StartHttpServer();
            StartPeriodicWifiChecker();
        }

        public void StartHttpServer()
        {
            HttpServerCallback();
        }

        public void StartPeriodicWifiChecker()
        {
            WifiCheckerCallback();
        }

        public void HttpServerCallback()
        {

        }

        public void WifiCheckerCallback()
        {

        }

        public void StopHTTPServer()
        {

        }

        public void StopPeriodicTasks()
        {

        }

        public void StopFlow()
        {
            StopPeriodicTasks();
            StopHTTPServer();
        }
    }
}
