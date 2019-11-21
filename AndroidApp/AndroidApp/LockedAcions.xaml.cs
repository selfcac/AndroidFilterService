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

        

        private async void BtnLock_Clicked(object sender, EventArgs e)
        {
            await FilterUtils.TimeLock.onlyUnlockedAsync(async () =>
            {
                var pageDatePicker = new InputPages.pageLockedDatePicker();
                await Application.Current.MainPage.Navigation.PushAsync(pageDatePicker);
            });
        }
    }
}