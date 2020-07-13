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
    public class ProfileViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly IProfileService _profileService;
        private readonly IInboxService _inboxService;
        public ICommand LogoutCommand { protected set; get; }
        public ICommand SendMessageCommand { protected set; get; }

        public Action LoggedOut;
        public Action GoToConversation;

        void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public ProfileViewModel(User user)
        {
            _profileService = new ProfileService();
            _inboxService = new InboxService();

            LogoutCommand = new Command(Logout);
            SendMessageCommand = new Command(ResumeOrStartConversation);

            SetUserProfile(user);
        }

        bool itsMe;
        int userId;
        string username = string.Empty;
        string profilePicture = string.Empty;

        public bool ItsMe
        {
            get => itsMe;
            set
            {
                itsMe = value;
                OnPropertyChanged(nameof(ItsMe));
            }
        }

        public int UserId
        {
            get => userId;
            set
            {
                userId = value;
                OnPropertyChanged(nameof(UserId));
            }
        }

        public string Username
        {
            get => username;
            set
            {
                username = value;  
                OnPropertyChanged(nameof(Username));
            }
        }

        public string ProfilePicture
        {
            get => profilePicture;
            set
            {
                profilePicture = value;
                OnPropertyChanged(nameof(ProfilePicture));
            }
        }

        public ConversationVM Conversation { get; set; }

        void SetUserProfile(User user)
        {
            UserId = user.UserId;
            Username = user.Username;
            ProfilePicture = user.ProfilePicturePath;
            ItsMe = App.AuthenticatedUser.UserId == user.UserId;
        }

        async void ResumeOrStartConversation()
        {
            Conversation = await _inboxService.ResumeOrStartConversation(UserId, Username, ProfilePicture);

            GoToConversation();
        }

        void Logout()
        {
            SecureStorage.Remove("fixes_token");
            App.IsUserLoggedIn = false;
            App.AuthenticatedUser = null;
            LoggedOut();
        }

    }
}
