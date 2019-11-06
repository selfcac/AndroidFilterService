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
        private static WifiManager wifi;
        private static WifiReceiver wifiReceiver = new WifiReceiver();

        public static void SetupWifiScan(Context ctx)
        {
 
            // Get a handle to the Wifi
            wifi = (WifiManager)ctx.GetSystemService(Context.WifiService);

            // register the Broadcast receiver to get the list of Wifi Networks
            ctx.RegisterReceiver(wifiReceiver, new IntentFilter(WifiManager.ScanResultsAvailableAction));
        }

        public static void RequestScan()
        {
            try
            {
                // Requirements here: https://developer.android.com/guide/topics/connectivity/wifi-scan
                wifi.StartScan();
            }
            catch (System.Exception ex)
            {
                AndroidBridge.e(AndroidBridge.TAG, ex.ToString());
                AndroidBridge.WifiScanningCallbackSucess?.Invoke(null, false);
            }
        }

        class WifiReceiver : BroadcastReceiver
        {
            string wifiDesc(ScanResult wifi)
            {
                string name = wifi.Ssid;
                string mac_addr = wifi.Bssid;
                return string.Format("{0}>{1}", mac_addr, name );
            }

            public override void OnReceive(Context context, Intent intent)
            {
                IList<ScanResult> scanwifinetworks = wifi.ScanResults;
                List<string> allWifis = new List<string>();

                if (scanwifinetworks.Count > 0)
                {
                    foreach (ScanResult wifinetwork in scanwifinetworks)
                    {
                        allWifis.Add(wifiDesc(wifinetwork));
                    }

                }
                AndroidBridge.WifiScanningCallbackSucess?.Invoke(allWifis, true);
            }
        }
    }
}