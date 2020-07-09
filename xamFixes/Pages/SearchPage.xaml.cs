using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamFixes.Models;
using xamFixes.ViewModels;

namespace xamFixes.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SearchPage : ContentPage
    {
        public SearchPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            searchBarInput.Focus();
        }

        async private void UserTapped(object sender, ItemTappedEventArgs args)
        {
            User user = (User)args.Item;
            if (user != null)
                await Navigation.PushAsync(new Chat.ConversationPage(new SearchPageViewModel(user)));
        }

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

    }
}