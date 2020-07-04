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
using xamFixes.Repository.ViewModel;
using xamFixes.Services;

namespace xamFixes.ViewModels
{
    class NewConversationViewModel : INotifyPropertyChanged
    {

        private readonly IFriendService _friendService;
        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public NewConversationViewModel()
        {
            _friendService = new FriendService();

            GetFriendsList();
        }

        List<User> contacts = new List<User>();

        public List<User> Contacts
        {
            get => contacts;
            set
            {
                contacts = value;
                OnPropertyChanged(nameof(Contacts));
            }
        }

        async Task GetFriendsList()
        {
            try
            {
                Contacts = await _friendService.GetFriendsList(App.AuthenticatedUser.UserId);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

    }
}
