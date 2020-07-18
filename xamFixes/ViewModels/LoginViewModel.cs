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
            enabledLogin = true;
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

        private bool enabledLogin;
        public bool EnabledLogin
        {
            get { return enabledLogin; }
            set
            {
                enabledLogin = value;
                PropertyChanged(this, new PropertyChangedEventArgs("EnabledLogin"));
            }
        }

        private string buttonColor = "White";
        public string ButtonColor
        {
            get { return buttonColor; }
            set
            {
                buttonColor = value;
                PropertyChanged(this, new PropertyChangedEventArgs("ButtonColor"));
            }
        }

        private string loginButtonText = "Login";
        public string LoginButtonText
        {
            get { return loginButtonText; }
            set
            {
                loginButtonText = value;
                PropertyChanged(this, new PropertyChangedEventArgs("LoginButtonText"));
            }
        }

        async Task AttemptLogin()
        {
            try
            { 

                ButtonColor = "LightGray";
                LoginButtonText = "Loading..";
                EnabledLogin = false;
                bool valid = ValidateAttemptLogin();

                if (!valid)
                    DisplayError("Invalid username/password");

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
            catch(Exception e)
            {
                return;
            }
            finally
            {
                EnabledLogin = true;
                ButtonColor = "White";
                LoginButtonText = "Login";
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