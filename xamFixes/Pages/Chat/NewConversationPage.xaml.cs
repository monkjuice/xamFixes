using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace xamFixes.Pages.Chat
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewConversationPage : ContentPage
    {
        public NewConversationPage()
        {
            InitializeComponent();
        }

        private void ContactTapped(object sender, ItemTappedEventArgs e)
        {

        }
    }
}