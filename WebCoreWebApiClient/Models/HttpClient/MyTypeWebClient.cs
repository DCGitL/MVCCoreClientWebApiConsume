using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebCoreWebApiClient.Models.Http.Client
{
    public class MyTypeWebClient
    {
        public MyTypeWebClient(HttpClient client)
        {
            Client = client;
        }
        public HttpClient Client { get; set; }
    }
}
