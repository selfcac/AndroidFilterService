using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AndroidApp.FilterUtils
{
    class WifiPeriodicChecker
    {
        static readonly string TAG = typeof(WifiPeriodicChecker).Name.ToString();

        public static void InitWifiChecker()
        {
            AndroidBridge.WifiScanningCallback = WifiCheckerCallback;
        }

        public static void WifiCheckerCallback(List<string> wifiLists, TimeSpan? timeSinceLastScan, Exception ex)
        {
            if (ex != null)
            {
                // Dont even check, bad zone:
                FilteringObjects.isInWifiBlockZone = true;
                AndroidBridge.d(TAG, "Wifi in bad zone because exepction.\n" + ex.ToString());
            }
            else
            {
                TimeSpan actualTime = (timeSinceLastScan ?? TimeSpan.FromDays(1));
                if (actualTime > FilteringServiceFlow.WIFI_PERIOD)
                {
                    // Dont even check, bad zone:
                    FilteringObjects.isInWifiBlockZone = true;
                    AndroidBridge.d(TAG, "Wifi in bad zone because results are old (" + actualTime + ")");
                }
                else
                {
                    bool inBadZoneResult = true;
                    try
                    {
                        if (!File.Exists(Filenames.WIFI_POLICY.getAppPrivate()))
                            File.WriteAllText(Filenames.WIFI_POLICY.getAppPrivate(), "");

                        IEnumerable<string> currentRules = File.ReadLines(Filenames.WIFI_POLICY.getAppPrivate());
                        List<string> newRules = new List<string>();

                        inBadZoneResult = CheckBlacklistedWifi.WifiHelper.fastBlockZoneCheck(wifiLists, currentRules, out newRules,
                            (log) => { AndroidBridge.d(TAG, "[CheckBlacklistedWifi] " + log); }
                            );

                        File.WriteAllLines(Filenames.WIFI_POLICY.getAppPrivate(), newRules);

                    }
                    catch (Exception ex2)
                    {
                        AndroidBridge.e(TAG, ex2);
                        inBadZoneResult = false; // allow in case of exceptions...
                    }

                    if (FilteringObjects.isInWifiBlockZone != inBadZoneResult)
                        AndroidBridge.ToastIt("Changing blackzone to: " + inBadZoneResult);
                    FilteringObjects.isInWifiBlockZone = inBadZoneResult;
                }
            }
        }
        
    }
}
