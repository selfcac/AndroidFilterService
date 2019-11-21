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
	public partial class inputPasswordDialog : ContentPage
	{
        Action<string> _OnComplete = null;
        string _OnCompleteText = "";

		public inputPasswordDialog (Action<string> OnComplete, string OnCompleteText)
		{
            _OnComplete = OnComplete;
            _OnCompleteText = OnCompleteText;

			InitializeComponent ();
            btnSetPass.Text = OnCompleteText;
		}

        private void BtnTogglePass_Clicked(object sender, EventArgs e)
        {
            txtPassword.IsPassword = !txtPassword.IsPassword;
        }

        private async void BtnSetPass_Clicked(object sender, EventArgs e)
        {
            _OnComplete?.Invoke(txtPassword.Text);
            await Application.Current.MainPage.Navigation.PopAsync();
        }
    }
}