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
using xamFixes.Repository;

namespace xamFixes.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient client = new HttpClient();

        async public Task CloseDatabase()
        {
            var db = new FixesDatabase();
            await db.CloseDatabase();
        }

        public async Task<User> LoginAsync(AuthUser credentials)
        {
            try
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

                string body = JsonConvert.SerializeObject(credentials, Formatting.Indented);

                var stringTask = client.PostAsync(Base.baseURL + "/login", new StringContent(body, Encoding.UTF8, "application/json"));

                var msg = await stringTask;

                var jsonString = msg.Content.ReadAsStringAsync().Result.Replace("\\", "").Trim('"');

                var response = JsonConvert.DeserializeObject<Base.Response>(jsonString);

                if (msg.IsSuccessStatusCode == false || response.Error == true || response.Message == null)
                {
                    return null;
                }

                await SecureStorage.SetAsync("fixes_token", response.Message["_token"].ToString());

                return JsonConvert.DeserializeObject<User>(response.Message["User"].ToString());

            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<bool> RegisterAsync(AuthUser credentials)
        {
            try
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

                string body = JsonConvert.SerializeObject(credentials, Formatting.Indented);

                var stringTask = client.PostAsync(Base.baseURL + "/register", new StringContent(body, Encoding.UTF8, "application/json"));

                var msg = await stringTask;

                var jsonString = msg.Content.ReadAsStringAsync().Result.Replace("\\", "").Trim('"');

                var response = JsonConvert.DeserializeObject<Base.Response>(jsonString);

                if (msg.IsSuccessStatusCode == false || response.Error == true)
                {
                    return false;
                }

                return true;

            }
            catch (Exception e)
            {
                return false;
            }
        }

    }
}
