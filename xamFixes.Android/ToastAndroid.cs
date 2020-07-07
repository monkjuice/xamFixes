using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using xamFixes.Droid;
using xamFixes.Interfaces;

[assembly: Xamarin.Forms.Dependency(typeof(ToastAndroid))]
namespace xamFixes.Droid
{
    public class ToastAndroid : IToast
    {
        public ToastAndroid()
        {
        }

        public void ShortAlert(string message)
        {
            Toast.MakeText(Application.Context, message, ToastLength.Short).Show();
        }

        public void LongAlert(string message)
        {
            Toast.MakeText(Application.Context, message, ToastLength.Long).Show();
        }
    }
}