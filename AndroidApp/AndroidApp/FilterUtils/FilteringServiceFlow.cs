﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AndroidApp.FilterUtils
{
    class FilteringServiceFlow
    {
        public static readonly TimeSpan WIFI_PERIOD = TimeSpan.FromMinutes(1);
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

        public void StartPeriodicTasks()
        {
            int taskID = AndroidBridge.scheduleJob(
                null, null, WIFI_PERIOD,
                (onJobDone)  => {
                    AndroidBridge.StartWifiScanning();
                    onJobDone(false); // wants rescedule?
                }
                , null, null, null);
            if (taskID < 0)
            {
                AndroidBridge.e(TAG, "Failed to schedule jog.");
            }
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
