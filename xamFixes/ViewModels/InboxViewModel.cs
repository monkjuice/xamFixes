using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using xamFixes.Interfaces;
using xamFixes.Models;
using xamFixes.Services;

namespace xamFixes.ViewModels
{
    class InboxViewModel : INotifyPropertyChanged
    {

        private readonly IInboxService _inboxService;
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand OpenConversationCommand { protected set; get; }


        void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public InboxViewModel()
        {
            _inboxService = new InboxService();

            OpenConversationCommand = new Command(OpenConversation);

            GetLastConversations();
        }

        List<Message> messages = new List<Message>();


        public List<Message> Messages
        {
            get => messages;
            set
            {
                messages = value;
                OnPropertyChanged(nameof(Messages));
            }
        }

        async Task GetLastConversations()
        {
            try
            {
                Messages = await _inboxService.GetLastConversations();
                Console.WriteLine(messages);
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        void OpenConversation()
        {
            return;
        }

    }
}
