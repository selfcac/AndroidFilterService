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
        const string TAG = "WIFI_SCAN";
        private static WifiManager myWifiManager =  null;

        public static void InitWifiScan(Context ctx)
        {
            try
            {
                myWifiManager = (WifiManager)ctx.GetSystemService(Context.WifiService);
            }
            catch (Exception ex)
            {
                MyLogger.e(TAG, ex);
            }
        }

        public static void RequestScan(Context ctx)
        {
            try
            {
                // register the Broadcast receiver to get the list of Wifi Networks
                ctx.RegisterReceiver(new WifiReceiver(), new IntentFilter(WifiManager.ScanResultsAvailableAction));

                // Requirements here: https://developer.android.com/guide/topics/connectivity/wifi-scan
                if (!myWifiManager?.StartScan() ?? false)
                {
                    AndroidBridge.WifiScanningCallback?.Invoke(null, new Exception("Starting scan failed quietly"));
                }
            }
            catch (Exception ex)
            {
                MyLogger.e(TAG, ex.ToString());
                AndroidBridge.WifiScanningCallback?.Invoke(null, ex);
            }
        }


        class WifiReceiver : BroadcastReceiver
        {
            DateTime start;
            public WifiReceiver ()
            {
                start = DateTime.Now;
            }

            string wifiDesc(ScanResult wifi)
            {
                string name = wifi.Ssid;
                string mac_addr = wifi.Bssid;
                return string.Format("{0}>{1}", mac_addr, name );
            }

            public override void OnReceive(Context context, Intent intent)
            {
                try
                {
                    IList<ScanResult> scanwifinetworks = myWifiManager.ScanResults;
                    List<string> allWifis = new List<string>();

                    if (DateTime.Now - this.start > TimeSpan.FromSeconds(20))
                    {
                        AndroidUtils.ToastIt(context, "Long wifi scan!!!");
                    }

                    // Ignore if fail, we get many in between scans
                    if (intent.GetBooleanExtra(WifiManager.ExtraResultsUpdated, false))
                    {
                        if (scanwifinetworks.Count > 0)
                        {
                            foreach (ScanResult wifinetwork in scanwifinetworks)
                            {
                                allWifis.Add(wifiDesc(wifinetwork));
                            }

                        }
                        AndroidBridge.WifiScanningCallback?.Invoke(allWifis, null);    
                    }
                }
                catch (Exception ex)
                {
                    MyLogger.e(TAG, ex);
                }


                // Try to stop scan in a different "try\e"
                try
                {
                    //https://stackoverflow.com/questions/4499915/how-to-stop-wifi-scan-on-android
                    context.UnregisterReceiver(this);
                    InvokeAbortBroadcast();
                }
                catch (Exception ex)
                {
                    MyLogger.e(TAG, ex);
                }

            }
        }
    }
}