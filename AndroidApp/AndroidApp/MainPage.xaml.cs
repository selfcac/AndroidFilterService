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

        static readonly string TAG = typeof(MainPage).Name.ToString();

        public MainPage()
        {
            InitializeComponent();
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

        int counter = 0;

        public int scheduleRepeated()
        {

            return AndroidBridge.scheduleJob(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10), null,
               (finishFunc) => {
                   if (counter < 6)
                   {
                       counter++;
                       AndroidBridge.ToastItFromBack("Counter: " + counter);
                       AndroidBridge.d(TAG,"Task counter: " + counter);
                       scheduleRepeated();
                   }
                   finishFunc(true);
               },
               () => counter < 6,
               null,
               () => false
               );
        }
     
        private void TestTask_Clicked(object sender, EventArgs e)
        {
            int id = scheduleRepeated();

            if (id > -1)
            {
                AndroidBridge.ToastIt("Task started");
            }
            else
            {
                AndroidBridge.ToastIt("Task failed");
            }
        }
    }
}
