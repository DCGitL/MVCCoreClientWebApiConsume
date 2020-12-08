using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebCoreWebApiClient.Controllers.Helper;
using WebCoreWebApiClient.Infrastructure.PartialRenderString;
using WebCoreWebApiClient.Models;
using WebCoreWebApiClient.ViewModel;

namespace WebCoreWebApiClient.Controllers
{

    public class DiscoverApiRoutesController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IPartialRender partialRender;

        public DiscoverApiRoutesController(IHttpClientFactory httpClientFactory, IPartialRender partialRender)
        {
            this.httpClientFactory = httpClientFactory;
            this.partialRender = partialRender;
        }
        public IActionResult Index()
        {
            return RedirectToAction(nameof(GetAllApiRoutes));
        }


        [HttpGet]
        public async Task<IActionResult> GetAllApiRoutes()
        {
            var model = new ApiRouteViewModel();

            var result = await DiscoverApiRouteHelper.GetApiList(httpClientFactory);
            if (result != null)
            {
                model.ApiRouteList = result.ToList();
                model.ApiVerbs = result.Select(v => v.verb).Distinct().ToList();
            }
            else
            {
                ModelState.AddModelError("", "Error obtaining data;");
            }

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> PostFilteredRoutes(string verbs)
        {
            var _msg = "No data to render html";
            var _html = string.Empty;
            var result = await DiscoverApiRouteHelper.GetApiList(httpClientFactory, verbs);
            if (result != null)
            {
                var model = result.ToList();
                try
                {
                    _html = await partialRender.RenderToStringAsync("DiscoverApiRoutes/PartialApiRoutes", model);

                }
                catch(Exception ex)
                {
                    throw ex;
                }
               
                _msg = string.Empty;
            }


            return Json(new { html = _html, msg = _msg });


        }

        
        [HttpPost]
        public IActionResult PostRouteList([FromBody] List<ApiEndPoint> apiendpoints)
        {


            return Json(new { msg = "Successfully updated" });
        }
            
    }
}
