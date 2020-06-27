using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using xamFixes.Interfaces;
using xamFixes.Services;

namespace xamFixes.ViewModels
{
    class ProfileViewModel : INotifyPropertyChanged
    {

        private readonly IProfileService _profileService;
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand LogoutCommand { protected set; get; }
        public Action LoggedOut;


        void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public ProfileViewModel()
        {
            _profileService = new ProfileService();

            LogoutCommand = new Command(Logout);

            if (_username == string.Empty)
                _ = GetUserProfile();
        }

        string _username = string.Empty;
        string _profilePicture = string.Empty;

        public string Username
        {
            get => _username;
            set
            {
                _username = value;  
                OnPropertyChanged(nameof(Username));
            }
        }

        public string ProfilePicture
        {
            get => _profilePicture;
            set
            {
                _profilePicture = value;
                OnPropertyChanged(nameof(ProfilePicture));
            }
        }

        async Task GetUserProfile()
        {
            var profile = await _profileService.GetUserProfile();

            Username = profile.Username;
            ProfilePicture = profile.ProfilePicturePath;
        }

        void Logout()
        {
            App.IsUserLoggedIn = false;
            SecureStorage.Remove("fixes_token");
            LoggedOut();
        }

    }
}
