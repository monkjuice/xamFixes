using System;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamFixes.Models;
using xamFixes.ViewModels;

namespace xamFixes.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InboxPage : ContentPage
    {
        public InboxPage()
        {
            InitializeComponent();
        }

        private void LoadTestMessages(object sender, EventArgs e)
        {
            var t = new xamFixes.Tests.SeedDummyData();
            t.Execute();
        }

        private void CopyDatabase(object sender, EventArgs e)
        {
            string outputPath = "/sdcard/" + App.AuthenticatedUser.UserId + ".db3";
            string dbPath = Path.Combine("/data/data/com.companyname.xamfixes/files/.local/share", App.AuthenticatedUser.UserId + ".db3");

            if (File.Exists(outputPath))
                File.Delete(outputPath);
            System.IO.File.Copy(dbPath, outputPath);
        }

        async private void ConversationTapped(object sender, ItemTappedEventArgs args)
        {
            ConversationVM conversation = (ConversationVM)args.Item;
            if (conversation != null)
                await Navigation.PushAsync(new Chat.ConversationPage(new ConversationViewModel(conversation)));
        }

        async private void NewConversation(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Chat.NewConversationPage());
        }

    }
}