using System;
using System.Collections.Generic;
using System.Text;

namespace AndroidApp.FilterUtils
{
    class FilteringServiceFlow
    {

        static readonly string TAG = typeof(FilteringServiceFlow).Name.ToString();


        public void StartFlow()
        {
            if (!AndroidBridge.isForegroundServiceUp())
            {
                StartHttpServer();
                StartPeriodicWifiChecker();
            }
            else
            {
                AndroidBridge.ToastIt("Service already up!");
            }
        }

        public void StartHttpServer()
        {
            HttpFilteringServer.HttpCallbackRouter();
        }

        public void StartPeriodicWifiChecker()
        {
            WifiCheckerCallback();
        }

        public void WifiCheckerCallback()
        {
            // How to avoid when hanging (assume bad zone)
        }

       // ============================================

        public void StopFlow()
        {
            if (AndroidBridge.isForegroundServiceUp())
            {
                StopPeriodicTasks();
                StopHTTPServer();
            }
            else
            {
                AndroidBridge.ToastIt("Service already stopped");
            }
        }

        public void StopHTTPServer()
        {

        }

        public void StopPeriodicTasks()
        {

        }
    }
}
