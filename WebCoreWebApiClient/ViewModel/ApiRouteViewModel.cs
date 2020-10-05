using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebCoreWebApiClient.Models;

namespace WebCoreWebApiClient.ViewModel
{
    public class ApiRouteViewModel
    {
        public ApiRouteViewModel()
        {
            ApiRouteList = new List<ApiRouteInfo>();
        }
        public List<ApiRouteInfo> ApiRouteList { get; set; }

        public List<string> ApiVerbs { get; set; }
    }
}
