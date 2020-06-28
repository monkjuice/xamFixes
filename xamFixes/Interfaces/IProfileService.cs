﻿using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using xamFixes.Models;

namespace xamFixes.Interfaces
{
    public interface IProfileService
    {
        Task<User> GetUserProfile();

        Task<bool> UploadProfilePicture(MediaFile picture);

    }
}
