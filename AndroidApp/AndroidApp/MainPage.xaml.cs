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

      

        private async void BtnOpenTest_Clicked(object sender, EventArgs e)
        {
            await Application.Current.MainPage.Navigation.PushAsync(new TestPage());
        }

        private void BtnAllowedPage_Clicked(object sender, EventArgs e)
        {

        }

        private void BtnUnlockPage_Clicked(object sender, EventArgs e)
        {

        }

        private void BtnLockedPage_Clicked(object sender, EventArgs e)
        {

        }
    }
}
