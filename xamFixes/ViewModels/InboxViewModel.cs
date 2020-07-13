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
using xamFixes.Crypto;
using xamFixes.Interfaces;
using xamFixes.Models;
using xamFixes.Services;
using xamFixes.Services.Utils;

namespace xamFixes.ViewModels
{
    class InboxViewModel : INotifyPropertyChanged
    {

        private readonly IInboxService _inboxService;
        private readonly IProfileService _profileService;

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public InboxViewModel()
        {
            _inboxService = new InboxService();
            _profileService = new ProfileService();

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

            hubConnection.On<byte[], string>("RecieveMessage", async (message, userid) =>
            {
                try 
                { 
                    var convo = await _inboxService.FindConversation(int.Parse(userid));

                    var user = await _profileService.GetUserProfile(int.Parse(userid));

                    if (convo == null)
                        return;

                    string decrypted = Crypto.FixesCrypto.DecryptData(await SecureStorage.GetAsync(convo.ConversationId.ToString()), message);

                    var listViewConversation = Conversations.Where(x => x.ConversationId == convo.ConversationId).FirstOrDefault();

                    if (listViewConversation == null)
                    {
                        convo.MessageBody = decrypted;
                        Conversations.Add(convo);
                    }
                    else
                    {
                        listViewConversation.MessageBody = decrypted;
                    }


                    _ = _inboxService.StoreMessage(_inboxService.CreateMessage(decrypted, int.Parse(userid)), convo.ConversationId, convo.UserId);
                }
                catch(Exception e)
                {
                    return;
                }
            });

            hubConnection.On<string, string>("RecieveHandshake", async (publickey, who) =>
            {

                var conversation = await _inboxService.FindConversation(int.Parse(who));

                if (conversation == null)
                    conversation = await _inboxService.StoreConversation(int.Parse(who));

                var user = await _profileService.GetUserProfile(int.Parse(who));

                // mine
                KeyPair keypair = FixesCrypto.GenerateKeyPair();
                await SecureStorage.SetAsync(conversation.ConversationId.ToString(), keypair.PrivateKey);

                // RespondHandshake
                await hubConnection.InvokeAsync("RespondHandshake", user.Username, keypair.PublicKey);

                // theirs
                await SecureStorage.SetAsync(Base64Encoder.Base64Encode(user.Username), publickey);
            });

            hubConnection.On<string, string>("HandshakeResponse", async (publickey, who) =>
            {
                var user = await _profileService.GetUserProfile(int.Parse(who));

                await SecureStorage.SetAsync(Base64Encoder.Base64Encode(user.Username), publickey);
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
