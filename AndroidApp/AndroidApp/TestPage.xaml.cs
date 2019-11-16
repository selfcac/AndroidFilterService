using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AndroidApp
{
	
	public partial class TestPage : ContentPage
	{
        static readonly string TAG = typeof(TestPage).Name.ToString();

		public TestPage ()
		{
			InitializeComponent ();
		}

        int periodic_task_counter = 0;
        public int scheduleRepeated()
        {

            return AndroidBridge.scheduleJob(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10), null,
               (finishFunc) => {
                   if (periodic_task_counter < 6)
                   {
                       periodic_task_counter++;
                       AndroidBridge.ToastItFromBack("Counter: " + periodic_task_counter);
                       AndroidBridge.d(TAG, "Task counter: " + periodic_task_counter);
                       scheduleRepeated();
                   }
                   finishFunc(true);
               },
               () => periodic_task_counter < 6,
               null,
               () => false
               );
        }

        private void BtnPeriodic_Clicked(object sender, EventArgs e)
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

        private void BtnScanWifi_Clicked(object sender, EventArgs e)
        {
            AndroidBridge.StartWifiScanning();
        }
    }
}