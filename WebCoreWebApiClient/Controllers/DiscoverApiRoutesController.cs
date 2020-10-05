using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebCoreWebApiClient.Models;
using WebCoreWebApiClient.ViewModel;

namespace WebCoreWebApiClient.Controllers
{
    
    public class DiscoverApiRoutesController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        public DiscoverApiRoutesController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }
        public IActionResult Index()
        {
            return RedirectToAction(nameof(GetAllApiRoutes));
        }


        [HttpGet]
        public async Task<IActionResult> GetAllApiRoutes()
        {

            var client = httpClientFactory.CreateClient("MyWebApiClient");
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);

            var model = new ApiRouteViewModel();

            var result = await client.GetAsync("api/v2.2/DiscoverApiRoutes/GetAllApiRoutes");
            if(result.IsSuccessStatusCode)
            {
                var resultData = await result.Content.ReadAsAsync<IEnumerable<ApiRouteInfo>>();
                model.ApiRouteList = resultData.Where(o=> o.verb != null).OrderBy(o=>o.verb)
                                        .ThenBy(o=>o.parametercount).ToList();
                var verbslist = model.ApiRouteList.Where(v => v.verb != null).Select(v => v.verb).Distinct().ToList();
                model.ApiVerbs = verbslist;

            }
            else
            {
                ModelState.AddModelError("", $"Error obtaining data, errorcode :{result.StatusCode}");
            }

            return View(model);
        }
    }
}
