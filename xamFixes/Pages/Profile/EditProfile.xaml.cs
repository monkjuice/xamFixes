using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamFixes.ViewModels;

namespace xamFixes.Pages.Profile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditProfile : ContentPage
    { 

        public EditProfile()
        {
            InitializeComponent();
            var vm = new EditProfileViewModel();
            this.BindingContext = vm;
            vm.DisplayError += async (string msg) => await DisplayError(msg);

            //TakePicture.Clicked += async (sender, args) =>
            //{
            //    await CrossMedia.Current.Initialize();

            //    if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            //    {
            //        await DisplayAlert("No Camera", ":( No camera available.", "OK");
            //        return;
            //    }

            //    var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            //    {
            //        Directory = "Fixes",
            //        Name = DateTime.Now.ToString() + ".jpg"
            //    });

            //    if (file == null)
            //        return;

            //    vm.Picture = file;

            //    ImagePreview.Source = ImageSource.FromStream(() =>
            //    {
            //        var stream = file.GetStream();
            //        return stream;
            //    });
            //};

        }

        public async Task DisplayError(string msg)
        {
            await DisplayAlert("Error", msg, "OK");
        }

        private void MainPage(object sender, EventArgs e)
        {
            Application.Current.MainPage = new MainPage();
        }

    }
}