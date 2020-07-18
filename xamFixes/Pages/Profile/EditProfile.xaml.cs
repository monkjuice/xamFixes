using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamFixes.ViewModels;

namespace xamFixes.Pages.Profile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditProfile : ContentPage
    { 

        public EditProfile()
        {
            InitializeComponent();
            var vm = new EditProfileViewModel();
            this.BindingContext = vm;
            vm.DisplayError += async (string msg) => await DisplayError(msg);
            vm.MainPage += () => MainPage();

        }

        public async Task DisplayError(string msg)
        {
            await DisplayAlert("Error", msg, "OK");
        }

        private void Cancel_Clicked(object sender, EventArgs e)
        {
            Application.Current.MainPage = new MainPage();
        }

        private void MainPage()
        {
            Application.Current.MainPage = new MainPage();
        }

    }
}