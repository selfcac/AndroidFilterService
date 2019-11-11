using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AndroidApp
{
    public partial class MainPage : ContentPage
    {
        const string TAG = "MainXaml";

        public MainPage()
        {
            InitializeComponent();
            SetupAndroidBridge();

        }

        private static void SetupAndroidBridge()
        {
            AndroidBridge.WifiScanningCallback = new Action<List<string>, Exception>((list, ex) =>
            {
                if (ex == null)
                {
                    string wifiInfo = string.Format(
                        "Sucess? {0} Got {1}, First: {2}",
                        (ex == null),
                        list?.Count ?? -1,
                        (list != null && list.Count > 0) ? list[0] : "<none>"
                    );
                    AndroidBridge.d(TAG, wifiInfo);
                    AndroidBridge.ToastIt(wifiInfo);
                }
                else
                {
                    AndroidBridge.e(TAG, ex);
                    AndroidBridge.ToastIt("Wifi failed: '" + ex.Message + "'");
                }

                // TODO:
                /*
                 * Periodic job to check wifi zone (start+stop while service live)
                 *      * Check if theter
                 * Test wifi zones
                 * Web server + view why page ( + check api)
                 * URL + time import export policies
                 * Lock\Unlocked
                 * System info (wifi zone, lock time, service state, etc...)
                */

            });

            AndroidBridge.OnForgroundServiceStart = new Action(() => { AndroidBridge.ToastIt("Service Start"); });
            AndroidBridge.OnForgroundServiceStop = new Action(() => { AndroidBridge.ToastIt("Service Stop"); });
            AndroidBridge.OnServiceInfoRequest = new Func<string>(() => "Some info!");
        }

        private void BtnScan_Clicked(object sender, EventArgs e)
        {
            AndroidBridge.StartWifiScanning();
        }

        private void BtnRestartService_Clicked(object sender, EventArgs e)
        {
            AndroidBridge.StopForgroundService();
            AndroidBridge.StartForgroundService();
        }

        private void BtnStopService_Clicked(object sender, EventArgs e)
        {
            AndroidBridge.StopForgroundService();
        }

        private void BtnStartService_Clicked(object sender, EventArgs e)
        {
            AndroidBridge.StartForgroundService();
        }
    }
}
