using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
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

            hubConnection.On<string, string>("RecieveMessage", async (message, userid) =>
            {
                try 
                {
                    var parsedMsg = JsonConvert.DeserializeObject<MessageVM>(message);

                    var convo = await _inboxService.FindConversation(int.Parse(userid));

                    var listViewConversation = Conversations.Where(x => x.ConversationId == convo.ConversationId).FirstOrDefault();

                    var user = await _profileService.GetUserProfile(int.Parse(userid));

                    string decrypted = Crypto.FixesCrypto.DecryptData(await SecureStorage.GetAsync(parsedMsg.PartialPublicKey), parsedMsg.EncryptedBody);

                     if (listViewConversation == null)
                    {
                        convo.MessageBody = decrypted;
                        Conversations.Add(convo);
                    }
                    else
                    {
                        listViewConversation.MessageBody = decrypted;
                        conversations.Remove(listViewConversation);
                        conversations.Insert(0, listViewConversation);
                        Conversations = conversations;
                    }

                    var prettyMsg = _inboxService.CreateMessage(decrypted, int.Parse(userid), parsedMsg.MessageId, parsedMsg.CreatedAt);

                    _ = _inboxService.StoreMessage(prettyMsg, convo.ConversationId, convo.UserId, true);

                    _ = hubConnection.InvokeAsync("RecievedMessage", user.Username, parsedMsg.MessageId);
                }
                catch(Exception e)
                {
                    return;
                }
            });

            hubConnection.On<string, string>("RecieveHandshake", async (publickey, who) =>
            {
                try
                {
                    var conversation = await _inboxService.FindConversation(int.Parse(who));

                    if (conversation == null)
                        conversation = await _inboxService.StoreConversation(int.Parse(who));

                    var user = await _profileService.GetUserProfile(int.Parse(who));

                    // mine
                    KeyPair keypair = FixesCrypto.GenerateKeyPair();
                    await SecureStorage.SetAsync(keypair.PublicKey.Substring(20,40), keypair.PrivateKey);
 
                    // RespondHandshake
                    await hubConnection.InvokeAsync("RespondHandshake", user.Username, keypair.PublicKey);

                    // theirs
                    await SecureStorage.SetAsync(conversation.ConversationId.ToString(), publickey);
                }
                catch(Exception e)
                {
                    return;
                }
            });

            hubConnection.On<string, string>("HandshakeResponse", async (publickey, who) =>
            {
                var conversation = await _inboxService.FindConversation(int.Parse(who));

                await SecureStorage.SetAsync(conversation.ConversationId.ToString(), publickey);

                //_ = SendUnsentMessages(int.Parse(who));

            });

            _ = hubConnection.On<string, string>("RecipientRecievedMessage", async (messageId, who) =>
              {
                  Guid id;
                  if (Guid.TryParse(messageId, out id))
                      _ = _inboxService.UpdateMessageStatus("RecievedMessage", id);
              });

            hubConnection.On<string, string>("RecipientReadMessage", async (messageId, who) =>
            {
                Guid id;
                if (Guid.TryParse(messageId, out id))
                    _ = _inboxService.UpdateMessageStatus("ReadMessage", id);
            });

        }

        async Task SendUnsentMessages(int who)
        {
            var msgs = await _inboxService.SendUnsentMessages(who);

            var to = await _profileService.GetUserProfile(who);

            var conversation = await _inboxService.ResumeOrStartConversation(who, to.Username, "");
            //var vm = null;
            
            //foreach (var msg in msgs)
            //{
            //    vm.UnsentBody = msg.Body;
            //    _ = vm.SendMessageCommand;
            //}
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
