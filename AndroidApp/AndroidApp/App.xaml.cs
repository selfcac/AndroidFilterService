using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace AndroidApp
{
    public partial class App : Application
    {
        static readonly string TAG = typeof(App).Name.ToString();

        public App()
        {
            InitializeComponent();

            // Toturial Here: https://www.youtube.com/watch?v=hqPl6uSHxYI&list=PLdo4fOcmZ0oU10SXt2W58pu2L0v2dOW-1&index=10
            MainPage = new NavigationPage(new MainPage());
        }


        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        protected override void OnStart()
        {
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

        }

        

    }
}
