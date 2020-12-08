using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using WebCoreWebApiClient.Models;

namespace WebCoreWebApiClient.Controllers.Helper
{
    public class DiscoverApiRouteHelper
    {
        public static async Task<IEnumerable<ApiRouteInfo>> GetApiList(IHttpClientFactory httpClientFactory, string verbs= "All")
        {
            var client = httpClientFactory.CreateClient("MyWebApiClient");
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);

            var result = await client.GetAsync("api/v2.2/DiscoverApiRoutes/GetAllApiRoutes");
            if (result.IsSuccessStatusCode)
            {
                var resultData = await result.Content.ReadAsAsync<IEnumerable<ApiRouteInfo>>();
                    if(verbs == "All")
                {
                    resultData = resultData.Where(o => o.verb != null).OrderBy(o => o.verb)
                                        .ThenBy(o => o.parametercount).ToList();
                }
                else
                {
                    resultData = resultData.Where(o => o.verb != null&& o.verb == verbs).OrderBy(o => o.verb)
                                        .ThenBy(o => o.parametercount).ToList();
                }
                   

                return resultData;
            }
            else
            {
                return null;
            }

        }
    }
}
