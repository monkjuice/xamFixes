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
    public class FriendService : IFriendService
    {
        private readonly HttpClient client = new HttpClient();

        async public Task<List<User>> GetFriendsList(int userId)
        {
            try
            {
                string _token = await SecureStorage.GetAsync("fixes_token");

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

                var stringTask = client.GetAsync(Base.baseURL + $"/api/friendship/friendslist?userId={userId}");

                var msg = await stringTask;

                var jsonString = msg.Content.ReadAsStringAsync().Result.Replace("\\", "").Trim('"');

                var response = JsonConvert.DeserializeObject<Base.Response>(jsonString);

                if (response.Error == true)
                {
                    return null;
                }

                return JsonConvert.DeserializeObject<List<User>>(response.Message["Friends"].ToString());

            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
