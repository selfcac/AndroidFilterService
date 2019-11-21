using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AndroidApp.InputPages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class pageLockedDatePicker : ContentPage
	{
		public pageLockedDatePicker ()
		{
			InitializeComponent ();
		}

        

        private async void BtnSet_Clicked(object sender, EventArgs e)
        {
            await FilterUtils.TimeLock.onlyUnlockedAsync(async () =>
            {
                DateTime PickedDate;
                PickedDate = datePicker1.Date;
                PickedDate = PickedDate.Add(timePicker1.Time - PickedDate.TimeOfDay);

                string result = FilterUtils.TimeLock.TrySetLockDate(PickedDate);
                AndroidBridge.ToastIt(result);

                await Application.Current.MainPage.Navigation.PopAsync();
            });
        }
    }
}