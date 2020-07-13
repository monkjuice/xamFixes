using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Markup;
using Xamarin.Forms.Xaml;
using xamFixes.Models;
using xamFixes.ViewModels;

namespace xamFixes.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProfilePage : ContentPage
    {

        ProfileViewModel _vm;

        public ProfilePage(ProfileViewModel vm)
        {
            InitializeComponent();
            _vm = vm;
            this.BindingContext = _vm;
            vm.LoggedOut += () => LoggedOut();
            vm.GoToConversation += () => GoToConversation();
        }

        private void LoggedOut()
        {
            Application.Current.MainPage = new LoginPage();
        }

        private void EditProfile_Clicked(object sender, EventArgs e)
        {
            Application.Current.MainPage = new Profile.EditProfile();
        }

        async private void GoToConversation()
        {
            await Navigation.PushAsync(new Chat.ConversationPage(new ConversationViewModel(_vm.Conversation)));
        }
    }
}