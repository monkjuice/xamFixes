using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
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
using xamFixes.Crypto;
using xamFixes.DBModel;
using xamFixes.Interfaces;
using xamFixes.Models;
using xamFixes.Services;
using xamFixes.Services.Utils;

namespace xamFixes.ViewModels
{
    public class ConversationViewModel : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        private HubConnection hubConnection;
        public Action ScrollToBottom;

        void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private readonly IInboxService _inboxService;
        private readonly IProfileService _profileService;

        ConversationVM _conversation;

        public Command SendMessageCommand { get; }

        public ConversationViewModel(ConversationVM conversation)
        {
            _inboxService = new InboxService();
            _profileService = new ProfileService();

            _conversation = conversation;

            recipientName = conversation.RecipientUsername;

            _ = GetConversationLastMessages();

            SendMessageCommand = new Command(async () => { await SendMessage(_conversation.RecipientUsername, UnsentBody); });

            _ = ConfigureHub().ConfigureAwait(false);

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

            await SendHandshake(_conversation.UserId);

            RecieveMessage();

            HandshakeResponse();
        }


        bool connectionEstablished = false;

        public bool ConnectionEstablished
        {
            get => connectionEstablished;
            set
            {
                connectionEstablished = value;
                OnPropertyChanged(nameof(ConnectionEstablished));
            }
        }

        bool RecipientIsOnline = false;

        string MyPublicKey;
        async Task SendHandshake(int who)
        {
            try 
            {
                ConnectionEstablished = false;

                KeyPair keypair = FixesCrypto.GenerateKeyPair();

                MyPublicKey = keypair.PublicKey;

                if(_conversation.ConversationId == Guid.Empty)
                    _conversation = await _inboxService.StoreConversation(who);

                _conversation.ConversationId = _conversation.ConversationId;

                await SecureStorage.SetAsync(MyPublicKey.Substring(20,40), keypair.PrivateKey);

                await hubConnection.InvokeAsync("SendHandshake", _conversation.RecipientUsername, MyPublicKey);

                if (await SuccessfulHandshake())
                {
                    ConnectionEstablished = true;
                }

                RecipientIsOnline = false;

                return;
            }
            catch(Exception e)
            {
                return;
            }
        }

        void RecieveMessage()
        {
            hubConnection.On<string, string>("RecieveMessage", async (message, userid) =>
            {

                var parsedMsg = JsonConvert.DeserializeObject<MessageVM>(message);

                string decrypted = Crypto.FixesCrypto.DecryptData(await SecureStorage.GetAsync(parsedMsg.PartialPublicKey), parsedMsg.EncryptedBody);

                var prettyMsg = _inboxService.CreateMessage(decrypted, int.Parse(userid), parsedMsg.MessageId, parsedMsg.CreatedAt);

                Messages.Add(prettyMsg);

                ScrollToBottom();

                _ = hubConnection.InvokeAsync("ReadMessage", _conversation.RecipientUsername, parsedMsg.MessageId);
            });
        }

        async public Task<bool> SuccessfulHandshake(int tries = 0)
        {
            if (tries > 5)
                return false;

            var exists = await SecureStorage.GetAsync(_conversation.ConversationId.ToString());

            if(exists != null)
                return true;
            else
            {
                Thread.Sleep(tries * 200);
                return await SuccessfulHandshake(tries += 1);
            }
        }

        public string publicKey { get; set; }

        async Task SendMessage(string who, string message)
        {
            EnabledSend = false;

            Guid messageId = Guid.NewGuid();

            try
            {
                bool isSent;
                //await SendHandshake(_conversation.UserId);
                //Thread.Sleep(100);
                if (RecipientIsOnline || ConnectionEstablished)
                {
                    string publicKey = await SecureStorage.GetAsync(_conversation.ConversationId.ToString());

                    var msg = new SecureMessage()
                    {
                        MessageId = messageId,
                        EncryptedBody = Crypto.FixesCrypto.EncryptText(publicKey, message),
                        CreatedAt = DateTime.Now.ToString(),
                        PartialPublicKey = publicKey.Substring(20, 40)
                    };

                    var jsonBody = JsonConvert.SerializeObject(msg);

                    if(RecipientIsOnline)
                    { 
                        // send 'live' message
                        _ = hubConnection.InvokeAsync("SendChatMessage", who, jsonBody);
                    }
                    else if (ConnectionEstablished)
                    {
                        // queue encrypted message
                        _ = _inboxService.QueueMessage("RecieveMessage", who, jsonBody);
                    }

                    isSent = true;
                }
                else
                {
                    // store message locally, await for public key
                    isSent = false;
                    _ = _inboxService.QueueMessage("RecieveHandshake", _conversation.RecipientUsername, MyPublicKey);
                }

                EnabledSend = true;

                var prettyMsg = _inboxService.CreateMessage(message, App.AuthenticatedUser.UserId, messageId, DateTime.Now);
                var result = await _inboxService.StoreMessage(prettyMsg, _conversation.ConversationId, _conversation.UserId, isSent);

                Messages.Add(prettyMsg);

                ScrollToBottom();

                UnsentBody = "";
            }
            catch(Exception e)
            {
                EnabledSend = true;
                return;
            }
        }

        void HandshakeResponse()
        {
            hubConnection.On<string, string>("HandshakeResponse", (publickey, who) =>
            {
                RecipientIsOnline = true;
            });
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
