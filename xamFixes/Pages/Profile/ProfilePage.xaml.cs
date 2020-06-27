﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamFixes.ViewModels;

namespace xamFixes.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProfilePage : ContentPage
    {
        public ProfilePage()
        {
            InitializeComponent();
            var vm = new ProfileViewModel();
            this.BindingContext = vm;
            vm.LoggedOut += () => LoggedOut();

        }

        private void LoggedOut()
        {
            Application.Current.MainPage = new LoginPage();
        }

        private void EditProfile_Clicked(object sender, EventArgs e)
        {
            Application.Current.MainPage = new Profile.EditProfile();
        }
    }
}