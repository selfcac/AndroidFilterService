using System;
using System.Collections.Generic;
using System.Text;

namespace AndroidApp.FilterUtils
{
    class FilteringServiceFlow
    {

        static readonly string TAG = typeof(FilteringServiceFlow).Name.ToString();

        HttpFilteringServer myHTTPServer = new HttpFilteringServer();
        WifiPeriodicChecker myWifiChecker = new WifiPeriodicChecker();

        public void StartFlow()
        {
            if (!AndroidBridge.isForegroundServiceUp())
            {
                AndroidBridge.OnForgroundServiceStart = () =>
                {
                    AndroidBridge.d(TAG, "Starting filtering flow");
                    myHTTPServer.StartHttpServer();
                    StartPeriodicTasks();
                };
                AndroidBridge.StartForgroundService();
            }
            else
            {
                AndroidBridge.ToastIt("Service already up!");
            }
        }

        public void StartPeriodicTasks()
        {
            myWifiChecker.WifiCheckerCallback();
        }

        public void StopFlow()
        {
            if (AndroidBridge.isForegroundServiceUp())
            {
                AndroidBridge.OnForgroundServiceStop = () =>
                {
                    AndroidBridge.d(TAG, "Stopping filtering flow");
                    StopPeriodicTasks();
                    myHTTPServer.StopHTTPServer();
                };
                AndroidBridge.StopForgroundService();
            }
            else
            {
                AndroidBridge.ToastIt("Service already stopped");
            }
        }

        public void StopPeriodicTasks()
        {
            AndroidBridge.stopAllJobs();
        }
    }
}
