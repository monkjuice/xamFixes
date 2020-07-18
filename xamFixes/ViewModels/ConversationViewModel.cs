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
            await hubConnection.StartAsync();

            await SendHandshake(_conversation.UserId);

            RecieveMessage();

            PersistHandshakeResponse();
        }


        bool connectionEstablished = false;

        public bool ConnectionEstablished
        {
            get => connectionEstablished;
            set
            {
                connectionEstablished = value; EnabledSend = value;
                OnPropertyChanged(nameof(ConnectionEstablished));
            }
        }

        async Task SendHandshake(int who)
        {
            try 
            {

                KeyPair keypair = FixesCrypto.GenerateKeyPair();

                if(_conversation.ConversationId == Guid.Empty)
                    _conversation = await _inboxService.StoreConversation(who);

                _conversation.ConversationId = _conversation.ConversationId;

                await SecureStorage.SetAsync(_conversation.ConversationId.ToString(), keypair.PrivateKey);

                await hubConnection.InvokeAsync("SendHandshake", _conversation.RecipientUsername, keypair.PublicKey);

                if (await SuccessfulHandshake(_conversation.RecipientUsername))
                {
                    ConnectionEstablished = true;
                }
            }
            catch(Exception e)
            {
                return;
            }
        }

        void RecieveMessage()
        {
            hubConnection.On<byte[], string>("RecieveMessage", async (message, userid) =>
            {
                string decrypted = Crypto.FixesCrypto.DecryptData(await SecureStorage.GetAsync(_conversation.ConversationId.ToString()), message);

                var prettyMsg = _inboxService.CreateMessage(decrypted, int.Parse(userid));

                Messages.Add(prettyMsg);

                ScrollToBottom();
            });
        }

        async public Task<bool> SuccessfulHandshake(string who, int tries = 0)
        {
            if (tries > 20)
                return false;

            var exists = await SecureStorage.GetAsync(Base64Encoder.Base64Encode(who));

            if(exists != null)
                return true;
            else
            {
                Thread.Sleep(tries * 100);
                return await SuccessfulHandshake(who, tries += 1);
            }
        }

        public string publicKey { get; set; }

        async Task SendMessage(string who, string message)
        {
            EnabledSend = false;

            byte[] encryptedMsg =  Crypto.FixesCrypto.EncryptText(await SecureStorage.GetAsync(Base64Encoder.Base64Encode(recipientName)), message);

            try 
            { 
                await hubConnection.InvokeAsync("SendChatMessage", who, encryptedMsg);

                EnabledSend = true;

                var prettyMsg = _inboxService.CreateMessage(message, App.AuthenticatedUser.UserId);
                var result = await _inboxService.StoreMessage(prettyMsg, _conversation.ConversationId, _conversation.UserId);

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

        void PersistHandshakeResponse()
        {
            hubConnection.On<string, string>("HandshakeResponse", async (publickey, who) =>
            {
                var user = await _profileService.GetUserProfile(int.Parse(who));

                await SecureStorage.SetAsync(Base64Encoder.Base64Encode(user.Username), publickey);

                ConnectionEstablished = true;
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
