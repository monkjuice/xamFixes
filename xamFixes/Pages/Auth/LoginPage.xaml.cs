using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamFixes.ViewModels;

namespace xamFixes.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();

            var vm = new LoginViewModel();
            this.BindingContext = vm;
            vm.DisplayError += async (string msg) => await DisplayError(msg);
            vm.LoggedIn += () => LoggedIn();

            username.Completed += (object sender, EventArgs e) =>
            {
                password.Focus();
            };

            password.Completed += (object sender, EventArgs e) =>
            {
                vm.SubmitCommand.Execute(null);
            };

        }

        public async Task DisplayError(string msg)
        {
            password.Text = String.Empty;
            await DisplayAlert("Error", msg, "OK");
        }

        public void LoggedIn()
        {
            App.IsUserLoggedIn = true;
            Application.Current.MainPage = new MainPage(); 
        }

        private void RegisterPage(object sender, EventArgs e)
        {
            Application.Current.MainPage = new RegisterPage();
        }
    }
}