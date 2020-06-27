using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamFixes.Interfaces;
using xamFixes.Services;

namespace xamFixes
{
    public partial class App : Application
    {
        public static bool IsUserLoggedIn { get; set; }


        public App()
        {
            InitializeComponent();

            var token = SecureStorage.GetAsync("fixes_token").Result;

            if (token != null)
            {
                IsUserLoggedIn = true;
            }
            else
            {
                IsUserLoggedIn = false;
            }

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
