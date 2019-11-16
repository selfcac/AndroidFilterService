using AndroidApp.FilterUtils;
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
	public partial class pageAllowedActions : ContentPage
	{
		public pageAllowedActions ()
		{
			InitializeComponent ();
		}

        private void BtnCheckLocked_Clicked(object sender, EventArgs e)
        {
            TaskResult unlockedStatus = TimeLock.isLocked();
            AndroidBridge.ToastIt(string.Format("Locked? {0}, Reason: '{1}'", unlockedStatus.success, unlockedStatus.eventReason));
        }
    }
}