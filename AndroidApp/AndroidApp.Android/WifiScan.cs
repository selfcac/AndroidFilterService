using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Net.Wifi;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;


namespace AndroidApp.Droid
{
    class WifiScan
    {

        static readonly string TAG = typeof(WifiScan).Name.ToString();
        private static WifiManager myWifiManager =  null;
        private static WifiReceiver myWifiReceiver = null;

        public static void getCurrentWifiStatus__NOT_USED__(Context ctx)
        {
            var wifi = (WifiManager)ctx.GetSystemService(Context.WifiService);
            WifiInfo info = wifi.ConnectionInfo;
        }

        public static void InitWifiScan(Context ctx)
        {
            try
            {
                myWifiManager = (WifiManager)ctx.GetSystemService(Context.WifiService);
                myWifiReceiver = new WifiReceiver();
            }
            catch (Exception ex)
            {
                AndroidLevelLogger.e(TAG, ex);
            }
        }

        public static void StartScanning(Context ctx)
        {
            try
            {
                // register the Broadcast receiver to get the list of Wifi Networks
                ctx.RegisterReceiver(myWifiReceiver, new IntentFilter(WifiManager.ScanResultsAvailableAction));

                // Requirements here: https://developer.android.com/guide/topics/connectivity/wifi-scan
                if (myWifiManager.IsWifiEnabled)
                {
                    if (!myWifiManager?.StartScan() ?? false)
                    {
                        AndroidBridge.WifiScanningCallback?.Invoke(null,null, new Exception("Starting scan failed quietly"));
                    }
                }
                else
                {
                    AndroidBridge.WifiScanningCallback?.Invoke(null,null, new Exception("Wifi is not on"));
                }

            }
            catch (Exception ex)
            {
                AndroidLevelLogger.e(TAG, ex.ToString());
                AndroidBridge.WifiScanningCallback?.Invoke(null,null, ex);
            }
        }


        public class WifiReceiver : BroadcastReceiver
        {
            public WifiReceiver ()
            {
            }

            static string wifiDesc(ScanResult wifi)
            {
                string name = wifi.Ssid;
                string mac_addr = wifi.Bssid;
                return string.Format("{0}>{1}", mac_addr, name );
            }

            public override void OnReceive(Context context, Intent intent)
            {

                // Try to stop scan in a different "try\e"
                try
                {
                    //https://stackoverflow.com/questions/4499915/how-to-stop-wifi-scan-on-android
                    context.UnregisterReceiver(this);

                    //InvokeAbortBroadcast not here:
                    //  https://medium.com/@ssaurel/develop-a-wifi-scanner-android-application-daa3b77feb73
                    //InvokeAbortBroadcast();
                }
                catch (Exception ex)
                {
                    AndroidLevelLogger.e(TAG, ex);
                }

                GetLatestWifiScanResults(intent.GetBooleanExtra(WifiManager.ExtraResultsUpdated, false));
                AndroidLevelLogger.d(TAG, "Wifi scan result sucess");
            }

            public static void GetLatestWifiScanResults(bool isUpToDate = true)
            {
                try
                {
                    IList<ScanResult> scanwifinetworks = myWifiManager.ScanResults;
                    List<string> allWifis = new List<string>();

                    // Ignore if fail, we get many in between scans
                    if (isUpToDate)
                    {
                        if (scanwifinetworks.Count > 0)
                        {
                            long timeOfScan = (scanwifinetworks.Max(s => s.Timestamp)) / 1000;
                            long currentTime = SystemClock.ElapsedRealtime();
                            TimeSpan timeSinceLastScan = TimeSpan.FromMilliseconds(currentTime - timeOfScan);

                            foreach (ScanResult wifinetwork in scanwifinetworks)
                            {
                                allWifis.Add(wifiDesc(wifinetwork));
                            }

                            AndroidBridge.WifiScanningCallback?.Invoke(allWifis, timeSinceLastScan, null);
                        }
                    }
                    
                }
                catch (Exception ex)
                {
                    AndroidLevelLogger.e(TAG, ex);

                    try
                    {
                        AndroidBridge.WifiScanningCallback?.Invoke(null,null, ex);
                    }
                    catch (Exception ex2)
                    {
                        AndroidLevelLogger.e(TAG, ex2);
                    }
                }
            }
        }
    }
}