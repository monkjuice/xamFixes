using System;
using System.Collections.Generic;
using System.Text;

namespace xamFixes.Services
{
    class Base
    {
        public static string baseURL = "https://fixesapi-dev.azurewebsites.net";

        public class Response
        {
            public string ResponseCode { get; set; }
            public Dictionary<string, object> Message { get; set; }
            public bool Error { get; set; }
            public Dictionary<string, string> ErrorList { get; set; }
        }
    }
}
