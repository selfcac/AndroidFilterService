using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AndroidApp.FilterUtils
{
    class FilteringServiceFlow
    {
        // Blocked state has slower freq, user has motivation to check manually to get out of blockzone
        public static readonly TimeSpan WIFI_PERIOD_BLOCKED = TimeSpan.FromMinutes(10); 
        public static readonly TimeSpan WIFI_PERIOD_ALLOWED = TimeSpan.FromMinutes(5);


        static readonly string TAG = typeof(FilteringServiceFlow).Name.ToString();

        HttpFilteringServer myHTTPServer = new HttpFilteringServer();
        WifiPeriodicChecker myWifiChecker = new WifiPeriodicChecker();

        public static FilteringServiceFlow INSTANCE = new FilteringServiceFlow();

        public void StartFlow()
        {
            try
            {
                // Reload policies:
                FilteringObjects.httpPolicy.reloadPolicy(File.ReadAllText(Filenames.HTTP_POLICY.getAppPrivate()));
                FilteringObjects.timePolicy.reloadPolicy(File.ReadAllText(Filenames.TIME_POLICY.getAppPrivate()));

                if (!AndroidBridge.isForegroundServiceUp())
                {
                    AndroidBridge.OnForgroundServiceStart = () =>
                    {
                        AndroidBridge.d(TAG, "Starting filtering flow");
                        myHTTPServer.StartHttpServer();
                        StartPeriodicTasks();
                    };

                    AndroidBridge.OnForgroundServiceStop = () =>
                    {
                        AndroidBridge.d(TAG, "Stopping filtering flow");
                        StopPeriodicTasks();
                        myHTTPServer.StopHTTPServer();
                    };

                    AndroidBridge.StartForgroundService();
                }
                else
                {
                    AndroidBridge.ToastIt("Service already up!");
                }
            }
            catch (Exception ex)
            {
                AndroidBridge.e(TAG, ex);
            }
        }

        public void scheduleRepeated()
        {
            TimeSpan WIFI_PERIOD = (FilteringObjects.isFiltering && FilteringObjects.isInWifiBlockZone) ?
                WIFI_PERIOD_BLOCKED : WIFI_PERIOD_ALLOWED;

            int taskID = AndroidBridge.scheduleJob(WIFI_PERIOD, WIFI_PERIOD + TimeSpan.FromSeconds(10), null,
               (finishFunc) => {
                   if (AndroidBridge.isForegroundServiceUp())
                   {
                       AndroidBridge.StartWifiScanning();
                       scheduleRepeated();
                   }
                   finishFunc(false);
               },
               () => AndroidBridge.isForegroundServiceUp(),
               null,
               () => false
               );
            if (taskID < 0)
            {
                AndroidBridge.e(TAG, "Failed to schedule job.");
            }
        }

        public void StartPeriodicTasks()
        {
            scheduleRepeated();
        }

        public void StopFlow()
        {
            if (AndroidBridge.isForegroundServiceUp())
            {
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
