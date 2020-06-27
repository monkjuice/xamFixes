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
    public class HomeService : IHomeService
    {
        private readonly HttpClient client = new HttpClient();

        async public Task<bool> Index()
        {
            try
            {
                string _token = await SecureStorage.GetAsync("fixes_token");

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

                var stringTask = client.GetAsync(Base.baseURL + "/api/home");

                var msg = await stringTask;

                if (msg.IsSuccessStatusCode)
                    return true;
                else
                    return false;

            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
