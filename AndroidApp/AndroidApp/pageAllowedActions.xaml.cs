using AndroidApp.FilterUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AndroidApp
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class pageAllowedActions : ContentPage
	{

        static readonly string TAG = typeof(pageAllowedActions).Name.ToString();

		public pageAllowedActions ()
		{
			InitializeComponent ();
		}

        private void BtnCheckLocked_Clicked(object sender, EventArgs e)
        {
            TaskResult unlockedStatus = TimeLock.isLocked();
            AndroidBridge.ToastIt(string.Format("Locked? {0}, Reason: '{1}'", unlockedStatus.success, unlockedStatus.eventReason));
        }

        private void BtnStartFiltering_Clicked(object sender, EventArgs e)
        {
            FilterUtils.FilteringObjects.isFiltering = true;
        }

        private void BtnExportFiles_Clicked(object sender, EventArgs e)
        {
            try
            {
                foreach (var policyPath in Filenames.EXPOSED_POLICIES)
                {
                    File.WriteAllText(policyPath.getAppPublic(), File.ReadAllText(policyPath.getAppPrivate()));
                }
            }
            catch (Exception ex)
            {
                AndroidBridge.e(TAG, ex);
                AndroidBridge.ToastIt("Export failed: " + ex.Message);
            }
        }

        private void BtnStartService_Clicked(object sender, EventArgs e)
        {
            FilteringServiceFlow.INSTANCE.StartFlow();
        }

        private void BtnStopService_Clicked(object sender, EventArgs e)
        {
            FilteringServiceFlow.INSTANCE.StopFlow();
        }


        private void BtnScanWifi_Clicked(object sender, EventArgs e)
        {
            AndroidBridge.StartWifiScanning();
        }
    }
}