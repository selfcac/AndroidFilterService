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

        protected override void OnAppearing()
        {
            base.OnAppearing();
            this.lblGitStatus.Text = GitInfo.GetInfo();
        }

        private async void BtnOpenTest_Clicked(object sender, EventArgs e)
        {
            await Application.Current.MainPage.Navigation.PushAsync(new TestPage());
        }

        private async void BtnAllowedPage_Clicked(object sender, EventArgs e)
        {
            await Application.Current.MainPage.Navigation.PushAsync(new pageAllowedActions());
        }

        private async void BtnUnlockPage_Clicked(object sender, EventArgs e)
        {
            await Application.Current.MainPage.Navigation.PushAsync(new pageUnlockOptions());
        }

        private async void BtnLockedPage_Clicked(object sender, EventArgs e)
        {
            await Application.Current.MainPage.Navigation.PushAsync(new LockedAcions());
        }

        private void BtnInfoPage_Clicked(object sender, EventArgs e)
        {
            try
            {
                lblInfo.Text = string.Format("[{0}] BlockZone? {1}, Filtering? {2}, Service up? {3}, Blocked now? {4}, Locked? {5} ",
                   DateTime.Now,
                   FilterUtils.FilteringObjects.isInWifiBlockZone,
                   FilterUtils.FilteringObjects.isFiltering,
                   AndroidBridge.isForegroundServiceUp(),
                   FilterUtils.FilteringObjects.timePolicy.isBlockedNow(),
                   FilterUtils.TimeLock.isLocked()
                   );
            }
            catch (Exception ex)
            {
                AndroidBridge.e(TAG, ex);
            }
        }
    }
}
