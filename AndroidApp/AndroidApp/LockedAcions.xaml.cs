using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AndroidApp
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LockedAcions : ContentPage
	{
        static readonly string TAG = typeof(LockedAcions).Name.ToString();

		public LockedAcions ()
		{
			InitializeComponent ();
		}

        private void BtnDebugUnlock_Clicked(object sender, EventArgs e)
        {
            FilterUtils.TimeLock.ForceUnlockNow();
        }

        private async void BtnLock_Clicked(object sender, EventArgs e)
        {
            await FilterUtils.TimeLock.onlyUnlockedAsync(async () =>
            {
                var pageDatePicker = new InputPages.pageLockedDatePicker();
                await Application.Current.MainPage.Navigation.PushAsync(pageDatePicker);
            });
        }

        private async void BtnStopService_Clicked(object sender, EventArgs e)
        {
            await FilterUtils.TimeLock.onlyUnlocked(() =>
            {
                AndroidBridge.StopForgroundService();
            });
        }

        private async void BtnSetMasterPass_Clicked(object sender, EventArgs e)
        {
            await FilterUtils.TimeLock.onlyUnlockedAsync(async () =>
            {
                var dialogPass = new InputPages.inputPasswordDialog((pass)=>
                {
                    var result = FilterUtils.MasterPassword.SetPassword(pass);
                    AndroidBridge.ToastIt(result.eventReason);
                }
                , "Set Password!"
                );
                await Application.Current.MainPage.Navigation.PushAsync(dialogPass);
            });
        }
    }
}