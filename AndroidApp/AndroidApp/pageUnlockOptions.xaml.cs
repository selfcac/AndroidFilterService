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
	public partial class pageUnlockOptions : ContentPage
	{
		public pageUnlockOptions ()
		{
			InitializeComponent ();
		}

        private async void BtnUnlockWithPass_Clicked(object sender, EventArgs e)
        {
            var dialogPass = new InputPages.inputPasswordDialog((pass) =>
                {
                    var result = FilterUtils.MasterPassword.ComparePass(pass);
                    if (result)
                    {
                        FilterUtils.TimeLock.ForceUnlockNow();
                    }
                    AndroidBridge.ToastIt(result.eventReason);
                }
                , "Unlock with pass!"
                );
            await Application.Current.MainPage.Navigation.PushAsync(dialogPass);
        }
    }
}