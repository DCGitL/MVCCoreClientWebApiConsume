using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebCoreWebApiClient.Models
{
    public class RequestRefreshToken
    {
        public string token { get; set; }
        public string refreshToken { get; set; }
    }
}
