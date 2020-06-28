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

namespace xamFixes.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient client = new HttpClient();

        public async Task<bool> LoginAsync(AuthUser credentials)
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

                if (msg.IsSuccessStatusCode == false || response.Error == true)
                {
                    return false;
                }

                await SecureStorage.SetAsync("fixes_token", response.Message["_token"].ToString());

                return true;

            }
            catch (Exception e)
            {
                return false;
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
