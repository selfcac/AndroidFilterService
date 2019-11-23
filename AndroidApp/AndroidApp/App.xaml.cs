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

            FilterUtils.WifiPeriodicChecker.InitWifiChecker();

            //AndroidBridge.WifiScanningCallback
            var a = new Action<List<string>,TimeSpan?, Exception>((list,timespan, ex) =>
            {
                if (ex == null)
                {
                    TimeSpan last = timespan ?? TimeSpan.FromDays(1);

                    string wifiInfo = string.Format(
                        "Sucess? {0} Got {1}, Time: {2}, First: {3}",
                        (ex == null),
                        list?.Count ?? -1,
                        last,
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
                 * System info (wifi zone, lock time, service state, etc...)
                */

            });

           

        }

        

    }
}
