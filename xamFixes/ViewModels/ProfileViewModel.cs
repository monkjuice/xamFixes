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

        private readonly IProfileService _profileService;
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand LogoutCommand { protected set; get; }
        public Action LoggedOut;


        void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public ProfileViewModel(User user)
        {
            _profileService = new ProfileService();

            LogoutCommand = new Command(Logout);

            SetUserProfile();

            Username = user.Username;
            ProfilePicture = user.ProfilePicturePath;
            ItsMe = App.AuthenticatedUser.UserId == user.UserId;
        }

        bool itsMe;
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

        void SetUserProfile()
        {
            Username = App.AuthenticatedUser.Username;
            ProfilePicture = App.AuthenticatedUser.ProfilePicturePath;
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
