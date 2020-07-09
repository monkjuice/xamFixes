using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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

        void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public InboxViewModel()
        {
            _inboxService = new InboxService();

            _ = ConfigureHub();

            _ = GetLastConversations();
        }

        async Task ConfigureHub()
        {
            hubConnection = new HubConnectionBuilder()
                                 .WithUrl($"https://fixesapi-dev.azurewebsites.net/chatHub", options =>
                                 {
                                     options.AccessTokenProvider = async () => await Task.FromResult(await SecureStorage.GetAsync("fixes_token"));
                                 })
                                 .Build();

            await hubConnection.StartAsync();

            hubConnection.On<string, string>("RecieveMessage", async (message, userid) =>
            {
                var convo = await _inboxService.FindConversation(int.Parse(userid));

                if (convo != null)
                {
                    var conversation = Conversations.Where(x => x.ConversationId == convo.ConversationId).First();

                    conversation.MessageBody = message;

                    _ = _inboxService.StoreMessage(_inboxService.CreateMessage(message, int.Parse(userid)), convo.ConversationId, convo.UserId);
                }
                else
                {
                    _ =  _inboxService.StoreMessage(_inboxService.CreateMessage(message, int.Parse(userid)), 0, int.Parse(userid));

                    convo = await _inboxService.CreateConversation(await _inboxService.FindConversation(int.Parse(userid)));

                    Conversations.Insert(0, await _inboxService.CreateConversation(convo));
                }
            });
        }

        ObservableCollection<ConversationVM> conversations = new ObservableCollection<ConversationVM>();
        private HubConnection hubConnection;

        public ObservableCollection<ConversationVM> Conversations
        {
            get => conversations;
            set
            {
                conversations = value;
                OnPropertyChanged(nameof(Conversations));
            }
        }

        async Task GetLastConversations()
        {
            try
            {
                Conversations = await _inboxService.GetLastConversations(App.AuthenticatedUser.UserId);
            }
            catch(Exception e)
            {
                throw e;
            }
        }

    }
}
