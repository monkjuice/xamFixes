using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using xamFixes.Interfaces;
using xamFixes.Models;
using xamFixes.Services;

namespace xamFixes.ViewModels
{
    public class ConversationViewModel : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        private HubConnection hubConnection;

        void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private readonly IInboxService _inboxService;

        ConversationVM _conversation;

        public Command SendMessageCommand { get; }

        public ConversationViewModel(ConversationVM conversation)
        {
            _inboxService = new InboxService();

            _conversation = conversation;

            recipientName = conversation.RecipientUsername;

            _ = GetConversationLastMessages();

            SendMessageCommand = new Command(async () => { await SendMessage(_conversation.RecipientUsername, UnsentBody); });

            _ = ConfigureHub();

        }

        async Task ConfigureHub()
        {
            hubConnection = new HubConnectionBuilder()
                                 .WithUrl($"https://fixesapi-dev.azurewebsites.net/chatHub", options =>
                                 {
                                     options.AccessTokenProvider = async () => await Task.FromResult(await SecureStorage.GetAsync("fixes_token"));
                                 })
                                 .Build();

            hubConnection.On<string, string>("RecieveMessage", (message, userid) =>
              {

                  var prettyMsg = _inboxService.CreateMessage(message, int.Parse(userid));

                  Messages.Add(prettyMsg);

              });

            await hubConnection.StartAsync();
        }

        async Task SendMessage(string who, string message)
        {
            EnabledSend = false;

            await hubConnection.InvokeAsync("SendChatMessage", who, message);

            EnabledSend = true;

            var prettyMsg = _inboxService.CreateMessage(message, App.AuthenticatedUser.UserId);
            var result = await _inboxService.StoreMessage(prettyMsg, _conversation.ConversationId, _conversation.UserId);
            _conversation.ConversationId = result.ConversationId;

            Messages.Add(prettyMsg);

            UnsentBody = "";
        }


        bool enabledSend = true;
        public bool EnabledSend
        {
            get => enabledSend;
            set
            {
                enabledSend = value;
                OnPropertyChanged(nameof(EnabledSend));
            }
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

        string _unsentBody;
        public string UnsentBody
        {
            get
            {
                return _unsentBody;
            }
            set
            {
                _unsentBody = value;
                OnPropertyChanged(nameof(UnsentBody));
            }
        }

        ObservableCollection<MessageVM> messages = new ObservableCollection<MessageVM>();

        public ObservableCollection<MessageVM> Messages
        {
            get => messages;
            set
            {
                messages = value;
                OnPropertyChanged(nameof(Messages));
            }
        }

        async Task GetConversationLastMessages()
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
