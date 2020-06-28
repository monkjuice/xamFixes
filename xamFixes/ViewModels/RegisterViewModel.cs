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
    public class RegisterViewModel : INotifyPropertyChanged
    {
        public Action<string> DisplayError;
        public Action LoggedIn;
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand SubmitRegister { protected set; get; }

        private readonly IAuthService _authService;


        public RegisterViewModel()
        {
            SubmitRegister = new Command(async () => await Register());
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

        async Task Register()
        {

            bool valid = ValidateRegister();

            if (!valid)
                return;

            var registerForm = new AuthUser();

            registerForm.Username = username;
            registerForm.Password = password;

            if (await _authService.RegisterAsync(registerForm) == false)
            {
                DisplayError("Username already in use");
            }
            else
            {
                var credentials = new AuthUser();
                credentials.Username = registerForm.Username;
                credentials.Password = registerForm.Password;

                await _authService.LoginAsync(credentials);
                LoggedIn();
            }
        }

         bool ValidateRegister()
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