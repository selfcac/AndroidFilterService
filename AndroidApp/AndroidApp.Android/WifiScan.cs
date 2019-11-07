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
        private static WifiManager myWifiManager =  null;
        private static WifiReceiver wifiReceiver = new WifiReceiver();

        public static void InitWifiScan(Context ctx)
        {
            myWifiManager = (WifiManager)ctx.GetSystemService(Context.WifiService);
        }

        public static void RequestScan(Context ctx)
        {
            try
            {
                // register the Broadcast receiver to get the list of Wifi Networks
                ctx.RegisterReceiver(wifiReceiver, new IntentFilter(WifiManager.ScanResultsAvailableAction));

                // Requirements here: https://developer.android.com/guide/topics/connectivity/wifi-scan
                myWifiManager?.StartScan();
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
                IList<ScanResult> scanwifinetworks = myWifiManager.ScanResults;
                List<string> allWifis = new List<string>();

                //https://stackoverflow.com/questions/4499915/how-to-stop-wifi-scan-on-android
                InvokeAbortBroadcast();
                context.UnregisterReceiver(this);

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