using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AndroidApp.FilterUtils
{
    class WifiPeriodicChecker
    {
        static readonly string TAG = typeof(WifiPeriodicChecker).Name.ToString();
        public static bool showReason = false;

        public static void InitWifiChecker()
        {
            AndroidBridge.WifiScanningCallback = WifiCheckerCallback;
        }

        public static void WifiCheckerCallback(List<string> wifiLists, TimeSpan? timeSinceLastScan, Exception callbackEx)
        {
            string reason = "init";
            bool shouldLogDebug = false;

            if (callbackEx != null)
            {
                // Dont even check, bad zone:
                FilteringObjects.isInWifiBlockZone = true;

                reason = "callback with exepction.";
                shouldLogDebug = true;
            }
            else
            {
                TimeSpan actualTime = (timeSinceLastScan ?? TimeSpan.FromDays(1));
                if (actualTime > FilteringServiceFlow.WIFI_PERIOD_BLOCKED)
                {
                    // Dont even check, bad zone:
                    FilteringObjects.isInWifiBlockZone = true;

                    reason = "results are old, wifi off? (Time: " + actualTime + ")";
                    shouldLogDebug = true;
                }
                else
                {
                    bool inBadZoneResult = true;
                    try
                    {
                        if (!File.Exists(Filenames.WIFI_POLICY.getAppPrivate()))
                            File.WriteAllText(Filenames.WIFI_POLICY.getAppPrivate(), "");

                        IEnumerable<string> currentRules = File.ReadAllLines(Filenames.WIFI_POLICY.getAppPrivate());
                        List<string> newRules = new List<string>();

                        //TODO Reason what wifi blocked.

                        inBadZoneResult = CheckBlacklistedWifiStandard.WifiHelper.fastBlockZoneCheck(wifiLists, currentRules, out newRules,
                            (log) => {/* AndroidBridge.d(TAG, "[CheckBlacklistedWifi] " + log);*/ } ,out reason
                            );

                        File.WriteAllLines(Filenames.WIFI_POLICY.getAppPrivate(), newRules);
                    }
                    catch (Exception ex2)
                    {
                        AndroidBridge.e(TAG, ex2);
                        inBadZoneResult = false; // allow in case of exceptions...

                        reason = "Exception processing Wifi Algo";
                    }

                    if (!showReason && FilteringObjects.isInWifiBlockZone != inBadZoneResult)
                        AndroidBridge.ToastIt("Changing blackzone to: " + inBadZoneResult);

                    FilteringObjects.isInWifiBlockZone = inBadZoneResult;
                }
            }

            reason = "[Blockzone? " + FilteringObjects.isInWifiBlockZone + "] " + reason;

            if (showReason)
            {
                AndroidBridge.ToastIt(reason);
                showReason = false; // Reset it until user mark it again.
            }

            if (shouldLogDebug)
            {
                if (callbackEx != null)
                {
                    reason += "\nCallback Exception:\n" + callbackEx.ToString();
                }
                AndroidBridge.d(TAG, reason);
            }            
        }
        
    }
}
