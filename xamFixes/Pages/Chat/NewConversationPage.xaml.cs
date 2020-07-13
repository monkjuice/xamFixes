using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamFixes.Models;
using xamFixes.ViewModels;

namespace xamFixes.Pages.Chat
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewConversationPage : ContentPage
    {
        public NewConversationPage()
        {
            InitializeComponent();
        }

        async private void ContactTapped(object sender, ItemTappedEventArgs args)
        {
            User contact = (User)args.Item;

            var conversation = new ConversationVM() { ConversationId = Guid.Empty, UserId = contact.UserId, RecipientUsername = contact.Username, RecipientProfilePicturePath = contact.ProfilePicturePath };

            if (conversation != null)
                await Navigation.PushAsync(new Chat.ConversationPage(new ConversationViewModel(conversation)));
        }
    }
}