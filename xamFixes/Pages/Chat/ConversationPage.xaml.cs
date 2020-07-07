using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamFixes.ViewModels;

namespace xamFixes.Pages.Chat
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConversationPage : ContentPage
    {
        public ConversationPage(ConversationViewModel vm)
        {
            this.BindingContext = vm;
            InitializeComponent();
        }

        private void OpenChatCamera(object sender, System.EventArgs e)
        {

        }
    }
}