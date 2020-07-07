using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace xamFixes.ViewModels
{
    public class DiscoverViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public DiscoverViewModel()
        {
        }

        //ObservableCollection<Post> posts = new ObservableCollection<Post>();

        //public ObservableCollection<MessageVM> Messages
        //{
        //    get => messages;
        //    set
        //    {
        //        messages = value;
        //        OnPropertyChanged(nameof(Messages));
        //    }
        //}


    }
}
