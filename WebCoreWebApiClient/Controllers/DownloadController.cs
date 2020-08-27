using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebCoreWebApiClient.Infrastructure;
using WebCoreWebApiClient.Models.Http.Client;

namespace WebCoreWebApiClient.Controllers
{
    public class DownloadController : BaseController
    {
        private readonly MyTypeWebClient typeClient;
        private readonly IHttpContextAccessor sessionContext;

        public DownloadController(MyTypeWebClient typeClient, IHttpContextAccessor sessionContext)
        {
            this.typeClient = typeClient;
            this.sessionContext = sessionContext;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> DownloadFile()
        {
            string token = await GetTokenAsync(typeClient, sessionContext);
            if(string.IsNullOrEmpty(token))
            {
                return View("Unauthorized");
            }
            typeClient.Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            typeClient.Client.DefaultRequestHeaders.Add("accept", "text/csv+d");
            var result = await typeClient.Client.GetAsync("/api/v3.1/Employee/GetEmployeesJsXmlCsv");
            if (result.IsSuccessStatusCode)
            {
                var byteAray = await result.Content.ReadAsByteArrayAsync();

                return File(byteAray, "text/csv", "download.csv");

            }
            else if(result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {

                return View("Unauthorized");
            }
            else
            {
                return View("errorDownload");

            }

        }


        
    }
}