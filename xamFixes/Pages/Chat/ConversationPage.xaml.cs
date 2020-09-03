using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamFixes.ViewModels;
using System.Threading.Tasks;
using System.Linq;

namespace xamFixes.Pages.Chat
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConversationPage : ContentPage
    {
        public ConversationPage(ConversationViewModel vm)
        {
            this.BindingContext = vm;
            vm.ScrollToBottom += () => ScrollToBottom();
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            ScrollToBottom();
        }

        public void ScrollToBottom()
        {
            var item = MessagesList.ItemsSource.Cast<object>().LastOrDefault();
            MessagesList.ScrollTo(item, ScrollToPosition.End, true);
        }

        private void OpenChatCamera(object sender, System.EventArgs e)
        {

        }

    }
}