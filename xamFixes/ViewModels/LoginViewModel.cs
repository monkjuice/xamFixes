using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

using xamFixes.Interfaces;
using xamFixes.Services;
using xamFixes.Models;

namespace xamFixes.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        public Action<string> DisplayError;
        public Action LoggedIn;
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand SubmitCommand { protected set; get; }

        private readonly IAuthService _authService;


        public LoginViewModel()
        {
            SubmitCommand = new Command(async () => await AttemptLogin());
            _authService = new AuthService();
        }

        private string username;
        public string Username
        {
            get { return username; }
            set
            {
                username = value;
                PropertyChanged(this, new PropertyChangedEventArgs("username"));
            }
        }
        private string password;
        public string Password
        {
            get { return password; }
            set
            {
                password = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Password"));
            }
        }

        async Task AttemptLogin()
        {

            bool valid = ValidateAttemptLogin();

            if (!valid)
                return;

            var credentials = new AuthUser();

            credentials.Username = username;
            credentials.Password = password;

            var user = await _authService.LoginAsync(credentials);
            if (user == null)
            {
                DisplayError("Invalid username/password");
            }
            else
            {
                App.IsUserLoggedIn = true;
                App.AuthenticatedUser = user;

                LoggedIn();
            }
        }

         bool ValidateAttemptLogin()
        {
            if (Xamarin.Essentials.Connectivity.NetworkAccess == Xamarin.Essentials.NetworkAccess.None)
            {
                DisplayError("No internet Connection");
                return false;
            }

            return true;
        }

    }
}