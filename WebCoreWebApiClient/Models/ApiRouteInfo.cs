using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebCoreWebApiClient.Models
{
    public class ApiRouteInfo
    {
        public string controller { get; set; }
        public string name { get; set; }
        public string verb { get; set; }

        public string endpoint { get; set; }
        public int parametercount { get; set; }
    }
}
