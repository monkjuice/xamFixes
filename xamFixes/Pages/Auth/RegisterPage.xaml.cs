using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamFixes.Interfaces;
using xamFixes.ViewModels;

namespace xamFixes.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegisterPage : ContentPage
    {
        public RegisterPage()
        {
            InitializeComponent();

            var vm = new RegisterViewModel();
            this.BindingContext = vm;
            vm.DisplayError += async (string msg) => await DisplayError(msg);
            vm.Success += () => Success();

            username.Completed += (object sender, EventArgs e) =>
            {
                username.Focus();
            };

            password.Completed += (object sender, EventArgs e) =>
            {
                vm.SubmitRegister.Execute(null);
            };

        }

        public async Task DisplayError(string msg)
        {
            password.Text = String.Empty;
            await DisplayAlert("Error", msg, "OK");
        }

        public void Success()
        {
            DependencyService.Get<IToast>().LongAlert($"Welcome to Fixes {username.Text}, please login to continue!");

            Application.Current.MainPage = new LoginPage();
        }
    }
}