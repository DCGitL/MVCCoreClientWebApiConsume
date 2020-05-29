using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebCoreWebApiClient.Models.Http.Client;

namespace WebCoreWebApiClient.Controllers
{
    public class DownloadController : Controller
    {
        private readonly MyTypeWebClient typeClient;

        public DownloadController(MyTypeWebClient typeClient)
        {
            this.typeClient = typeClient;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> DownloadFile()
        {
            string token = GetToken();
            if(string.IsNullOrEmpty(token))
            {
                return View("Unauthorized");
            }
            typeClient.Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            typeClient.Client.DefaultRequestHeaders.Add("accept", "text/csv");
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


        private string GetToken()
        {
            ISession session = HttpContext.Session;
            var token = session.GetString("access_token");

            return token;
        }
    }
}