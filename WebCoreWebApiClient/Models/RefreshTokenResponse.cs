using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebCoreWebApiClient.Models
{
    public class RefreshTokenResponse
    {
        public string token { get; set; }
        public string refreshToken { get; set; }
        public bool success { get; set; }
        public List<string> errors { get; set; } 
        public DateTime? accessTokenExpiration { get; set; }
        public DateTime?  issuedDate { get; set; }


    }
}
