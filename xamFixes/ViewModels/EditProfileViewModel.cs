using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using xamFixes.Interfaces;
using xamFixes.Pages;
using xamFixes.Services;

namespace xamFixes.ViewModels
{
    class EditProfileViewModel : INotifyPropertyChanged
    {

        public Action<string> DisplayError;

        private readonly IProfileService _profileService;
        public ICommand SubmitCommand { protected set; get; }
        public ICommand ChoosePictureCommand { protected set; get; }
        public ICommand TakePictureCommand { protected set; get; }

        public MediaFile Picture { get; set; }

        public EditProfileViewModel()
        {

            SubmitCommand = new Command(async () => await UploadProfilePicture());

            ChoosePictureCommand = new Command(async () => await ChoosePicture());

            TakePictureCommand = new Command(async () => await TakePicture());

            _profileService = new ProfileService();
        }

        private bool previewingPicture = false;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool PreviewingPicture
        {
            get { return previewingPicture; }
            set
            {
                previewingPicture = value;
                PropertyChanged(this, new PropertyChangedEventArgs("PreviewingPicture"));
            }
        }

        ImageSource picturePreview;
        public ImageSource PicturePreview
        {
            get { return picturePreview; }
            set
            {
                picturePreview = value;
                PropertyChanged(this, new PropertyChangedEventArgs("PicturePreview"));
            }
        }

        async Task UploadProfilePicture()
        {
            var success = await _profileService.UploadProfilePicture(Picture);
            return;
        }

        async Task TakePicture()
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                DisplayError(":( No images library available.");
                return;
            }

            var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                Directory = "Fixes",
                Name = DateTime.Now.ToString() + ".jpg"
            });

            if (file == null)
                return;

            PreviewingPicture = true;

            Picture = file;

            PicturePreview = ImageSource.FromFile(file.Path);
        }

        async Task ChoosePicture()
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                DisplayError(":( No images library available.");
                return;
            }

            var file = await CrossMedia.Current.PickPhotoAsync();

            if (file == null)
                return;

            PreviewingPicture = true;

            Picture = file;

            PicturePreview = ImageSource.FromFile(file.Path);
        }

    }
}
