using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using WebCoreWebApiClient.CustomAttributes;
using WebCoreWebApiClient.Infrastructure;
using WebCoreWebApiClient.Models;
using WebCoreWebApiClient.Models.Http.Client;

namespace WebCoreWebApiClient.Controllers
{
    //[AuthorizationActionFilter]
    [Authorize(Roles ="Admin")]
    public class EmployeeController : BaseController
    {
      
        private readonly MyTypeWebClient TypeClient;

        public readonly IHttpContextAccessor SessionContext;

        public  EmployeeController(MyTypeWebClient typeClient, IHttpContextAccessor sessionContext)
        {
            TypeClient = typeClient;
            SessionContext = sessionContext;
           
        }

        public IActionResult Index() => RedirectToAction("GetEmployees");



        public async Task<IActionResult> Create()
        {
            string token = await GetTokenAsync(TypeClient, SessionContext);

           
            return View(new Employee());

        }

        [HttpPost]
        public async Task<IActionResult> Create(Employee employee)
        {
            if (ModelState.IsValid)
            {
                var token = await GetTokenAsync(TypeClient, SessionContext);
               
                TypeClient.Client.DefaultRequestHeaders.Accept.Clear();

                TypeClient.Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                string content = JsonConvert.SerializeObject(employee);
                var buffer = Encoding.UTF8.GetBytes(content);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = await TypeClient.Client.PostAsync("/api/v3.1/EmployeeDb/CreateEmployee", byteContent).ConfigureAwait(false);
                string result = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.OK)
                {
                    ModelState.AddModelError("", $"Fail in process request status code {response.StatusCode}");
                }
                else
                {
                    var value = JsonConvert.DeserializeObject<Employee>(result);
                    return RedirectToAction("GetEmployees");
                }




            }

            return View(employee);


        }

        public async Task<IActionResult> GetEmployees()
        {
            var auth = User.Identity.IsAuthenticated;
            var username = User.Identity.Name;

            IEnumerable<Employee> employees = new List<Employee>();
            var token = await GetTokenAsync(TypeClient, SessionContext);
            //setup the header with token
            TypeClient.Client.DefaultRequestHeaders.Authorization =
             new AuthenticationHeaderValue("Bearer", token);

            TypeClient.Client.DefaultRequestHeaders.Add("Accept", "application/json");

            //HTTP GET
            var responseTask = await TypeClient.Client.GetAsync("/api/v3.1/EmployeeDb/GetAllDbEmployees");

            if (responseTask.IsSuccessStatusCode)
            {
                var readTask = await responseTask.Content.ReadAsAsync<IEnumerable<Employee>>();
                employees = readTask;
            }
            else
            {
                ModelState.AddModelError("", $"Error obtaining data, errorcode :{responseTask.StatusCode}");
            }

            ViewBag.message = TempData["message"] != null ? TempData["message"].ToString() : string.Empty;

            return View(employees);

        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {

            Employee employee = null;
            var token = await GetTokenAsync(TypeClient, SessionContext);

            TypeClient.Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            TypeClient.Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Http Get
            var getTask = await TypeClient.Client.GetAsync("/api/v3.1/EmployeeDb/GetDbEmployee/" + id.ToString());

            if (getTask.IsSuccessStatusCode)
            {
                var readTask = await getTask.Content.ReadAsAsync<Employee>();

                employee = readTask;

                return View(employee);
            }
            else
            {
                ModelState.AddModelError("", $"Process fail status Code is {getTask.StatusCode}");
                return View(new Employee());
            }


        }

        //Update the edited employee
        [HttpPost]
        public async Task<IActionResult> Edit(Employee employee)
        {
            if (ModelState.IsValid)
            {
                var token = await GetTokenAsync(TypeClient, SessionContext);

                TypeClient.Client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

                string content = JsonConvert.SerializeObject(employee);
                var buffer = Encoding.UTF8.GetBytes(content);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = await TypeClient.Client.PutAsync("/api/v3.1/EmployeeDb/UpdateDbEmployee", byteContent).ConfigureAwait(false);
                string result = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("GetEmployees");
                }
                else
                {
                    ModelState.AddModelError("", $"Error in processing the request {response.StatusCode}");
                    return View(employee);
                }


            }
            else
            {
                return View(employee);
            }


        }


        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var token = await GetTokenAsync(TypeClient, SessionContext);

            TypeClient.Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var deleteTask = await TypeClient.Client.DeleteAsync("/api/v3.1/EmployeeDb/DeleteDbEmployee/" + id.ToString());

            if (deleteTask.IsSuccessStatusCode)
            {
                TempData["message"] = "Transaction was successful";
            }
            else
            {
                TempData["message"] = $"Transaction fail status code {deleteTask.StatusCode}";
            }


            return RedirectToAction("GetEmployees");

        }


      
    }
}