using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using xamFixes.Interfaces;
using Xamarin.Essentials;
using xamFixes.Models;
using System.IO;
using Plugin.Media.Abstractions;

namespace xamFixes.Services
{
    public class ProfileService : IProfileService
    {
        private readonly HttpClient client = new HttpClient();

        async public Task<User> GetUserProfile()
        {
            try
            {
                string _token = await SecureStorage.GetAsync("fixes_token");

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

                var stringTask = client.GetAsync(Base.baseURL + "/api/user/profile");

                var msg = await stringTask;

                var jsonString = msg.Content.ReadAsStringAsync().Result.Replace("\\", "").Trim('"');

                var response = JsonConvert.DeserializeObject<Base.Response>(jsonString);

                if (response.Error == true)
                {
                    return null;
                }

                return JsonConvert.DeserializeObject<User>(response.Message["Profile"].ToString());

            }
            catch (Exception e)
            {
                return null;
            }
        }

        async public Task<bool> UploadProfilePicture(MediaFile picture)
        {
            try
            {
                string _token = await SecureStorage.GetAsync("fixes_token");

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

                var content = new MultipartFormDataContent();

                content.Add(new StreamContent(picture.GetStream()),
                    "Image",
                    picture.Path);

                var task = await client.PostAsync(Base.baseURL + "/api/user/uploadprofilepicture", content);

                if (!task.IsSuccessStatusCode)
                    return false;
                
                var jsonString = task.Content.ReadAsStringAsync().Result.Replace("\\", "").Trim('"');

                var response = JsonConvert.DeserializeObject<Base.Response>(jsonString);

                if (response.Error == true)
                    return false;

                picture.Dispose();

                return true;

            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
