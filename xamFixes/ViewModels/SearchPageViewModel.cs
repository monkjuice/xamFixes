using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using xamFixes.Interfaces;
using xamFixes.Models;
using xamFixes.Services;

namespace xamFixes.ViewModels
{
    public class SearchPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly IUserService _userService;

        public ICommand SearchUsersCommand { protected set; get; }


        void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public SearchPageViewModel(User user)
        {
            SearchUsersCommand = new Command(async () => await SearchUsers());
            _userService = new UserService();
        }

        async Task SearchUsers()
        {
            Users = await _userService.FindUsers(Username);
            System.Threading.Thread.Sleep(50);
        }

        string username { get; set; }

        public string Username
        {
            get => username;
            set
            {
                username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        ObservableCollection<User> users = new ObservableCollection<User>();
        private User user;

        public ObservableCollection<User> Users
        {
            get => users;
            set
            {
                users = value;
                OnPropertyChanged(nameof(Users));
            }
        }


    }
}
