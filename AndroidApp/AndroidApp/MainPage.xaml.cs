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
        public MainPage()
        {
            InitializeComponent();
            AndroidBridge.WifiScanningCallbackSucess = new Action<List<string>, bool>((list, sucess) =>
            {
                AndroidBridge.ToastIt(string.Format("Sucess? {0} Got {1}, First: {2}",
                    sucess,
                    list?.Count ?? -1,
                    (list != null && list.Count > 0) ? list[0] : "X"
                    ));
            });
        }

        private void BtnScan_Clicked(object sender, EventArgs e)
        {
            AndroidBridge.StartWifiScanning();
        }
    }
}
