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
using xamFixes.Repository.ViewModel;
using xamFixes.Services;

namespace xamFixes.ViewModels
{
    public class ConversationViewModel : INotifyPropertyChanged
    {
        private readonly IInboxService _inboxService;

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        ConversationVM _conversation;

        public ConversationViewModel(ConversationVM conversation)
        {
            _inboxService = new InboxService();

            _conversation = conversation;

            recipientName = conversation.RecipientUsername;

            _ = GetLastConversations();
        }

        string recipientName;

        public string RecipientName
        {
            get => recipientName;
            set
            {
                recipientName = value;
                OnPropertyChanged(nameof(RecipientName));
            }
        }

        List<MessageVM> messages = new List<MessageVM>();

        public List<MessageVM> Messages
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
                Messages = await _inboxService.GetConversationLastMessages(_conversation.ConversationId);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

    }
}
