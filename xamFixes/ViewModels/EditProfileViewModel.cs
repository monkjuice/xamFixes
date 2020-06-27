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
    class EditProfileViewModel
    {

        private readonly IProfileService _profileService;
        public ICommand SubmitCommand { protected set; get; }

        public MediaFile file { get; set; }

        public EditProfileViewModel()
        {
            SubmitCommand = new Command(async () => await UploadProfilePicture());
            _profileService = new ProfileService();
        }

        async Task UploadProfilePicture()
        {
            var success = await _profileService.UploadProfilePicture(file);
            return;
        }

    }
}
