﻿using System;
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
    class InboxViewModel : INotifyPropertyChanged
    {

        private readonly IInboxService _inboxService;
        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public InboxViewModel()
        {
            _inboxService = new InboxService();

            GetLastConversations();
        }

        List<ConversationVM> conversations = new List<ConversationVM>();

        public List<ConversationVM> Conversations
        {
            get => conversations;
            set
            {
                conversations = value;
                OnPropertyChanged(nameof(Conversations));
            }
        }

        async Task GetLastConversations()
        {
            try
            {
                Conversations = await _inboxService.GetLastConversations(App.AuthenticatedUser.UserId);
            }
            catch(Exception e)
            {
                throw e;
            }
        }

    }
}
