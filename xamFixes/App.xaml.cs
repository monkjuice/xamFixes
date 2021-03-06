﻿using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamFixes.Interfaces;
using xamFixes.Models;
using xamFixes.Repository;
using xamFixes.Services;

namespace xamFixes
{
    public partial class App : Application
    {
        public static bool IsUserLoggedIn { get; set; }

        public static User AuthenticatedUser { get; set; }

        bool devMode = true;

        public App()
        {

            if(devMode)
                SecureStorage.Remove("fixes_token");

            InitializeComponent();

            var token = SecureStorage.GetAsync("fixes_token").Result;

            if (token != null)
                IsUserLoggedIn = true;
            else
                IsUserLoggedIn = false;

            if (IsUserLoggedIn == false)
                MainPage = new Pages.LoginPage();
            else
                MainPage = new Pages.MainPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {

        }
    }
}
