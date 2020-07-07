using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using xamFixes.Models;

namespace xamFixes.ViewModels
{
    public class SearchPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public SearchPageViewModel()
        {
        }

        ObservableCollection<DiscoverSearchResult> results = new ObservableCollection<DiscoverSearchResult>();

        public ObservableCollection<DiscoverSearchResult> Messages
        {
            get => results;
            set
            {
                results = value;
                OnPropertyChanged(nameof(Messages));
            }
        }


    }
}
