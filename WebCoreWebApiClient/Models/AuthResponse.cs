
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebCoreWebApiClient.Models
{
    public class AuthResponse
    {
        public string accessToken { get; set; }
        public DateTime? expirationDateTime { get; set; }

        public DateTime? dateIssued { get; set; }
        public string refreshToken { get; set; }
    }
}
