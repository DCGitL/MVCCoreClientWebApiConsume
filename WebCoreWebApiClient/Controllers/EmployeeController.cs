using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebCoreWebApiClient.CustomAttributes;
using WebCoreWebApiClient.Models;

namespace WebCoreWebApiClient.Controllers
{
    [AuthorizationActionFilter]
    public class EmployeeController : Controller
    {
        private const string webapiUri = "http://webapiservices.com/";
        public IActionResult Index() => RedirectToAction("GetEmployees");
       


        public IActionResult Create() => View(new Employee());

        [HttpPost]
        public async Task<IActionResult> Create(Employee employee)
        {
            string token = GetSessionToken();
           

            if (ModelState.IsValid)
            {
                
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(webapiUri);

                    client.DefaultRequestHeaders.Accept.Clear();
                    
                  
                
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    string content = JsonConvert.SerializeObject(employee);
                    var buffer = Encoding.UTF8.GetBytes(content);
                    var byteContent = new ByteArrayContent(buffer);
                    byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    var response = await client.PostAsync("api/employee", byteContent).ConfigureAwait(false);
                    string result = await response.Content.ReadAsStringAsync();
                    if(response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.OK)
                    {
                        ModelState.AddModelError("", $"Fail in process request status code {response.StatusCode}");
                    }
                    else
                    {
                        var value = JsonConvert.DeserializeObject<Employee>(result);
                        return RedirectToAction("GetEmployees");
                    }
                   

                   
                }
            }

            return View(employee);


        }

        public async Task<IActionResult> GetEmployees()
        {
            IEnumerable<Employee> employees = new List<Employee>();

            string token = GetSessionToken();
           

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(webapiUri);
                //setup the header with token
                client.DefaultRequestHeaders.Authorization =
                 new AuthenticationHeaderValue("Bearer", token);

                //HTTP GET
                var responseTask = await client.GetAsync("api/employee");

                if (responseTask.IsSuccessStatusCode)
                {
                    var readTask = await responseTask.Content.ReadAsAsync<IEnumerable<Employee>>();
                    employees = readTask;
                }

                ViewBag.message = TempData["message"] != null? TempData["message"].ToString(): string.Empty;

                return View(employees);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            string token = GetSessionToken();
           
            Employee employee = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(webapiUri);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                //Http Get
                var getTask = await client.GetAsync("api/employee/" + id.ToString());

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
        }

        //Update the edited employee
        [HttpPost]
        public async Task<IActionResult> Edit(Employee employee)
        {
            string token = GetSessionToken();
           

            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(webapiUri);
                    client.DefaultRequestHeaders.Authorization =
                      new AuthenticationHeaderValue("Bearer", token);

                    string content = JsonConvert.SerializeObject(employee);
                    var buffer = Encoding.UTF8.GetBytes(content);
                    var byteContent = new ByteArrayContent(buffer);
                    byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    var response = await client.PutAsync($"api/employee/{employee.id}", byteContent).ConfigureAwait(false);
                    string result = await response.Content.ReadAsStringAsync();

                    if(response.IsSuccessStatusCode)                                   
                    {
                        return RedirectToAction("GetEmployees");
                    }
                    else
                    {
                        ModelState.AddModelError("", $"Error in processing the request {response.StatusCode}");
                        return View(employee);
                    }

                }
            }
            else
            {
                return View(employee);
            }


        }


        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            string token = GetSessionToken();
         

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(webapiUri);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var deleteTask = await client.DeleteAsync("api/employee/" + id.ToString());

                if (deleteTask.IsSuccessStatusCode)
                {
                    TempData["message"] = "Transaction was successful";
                }
                else
                {
                    TempData["message"] = $"Transaction fail status code {deleteTask.StatusCode}";
                }
            }

            return RedirectToAction("GetEmployees");

        }


        private string GetSessionToken()
        {
            ISession session = HttpContext.Session;
            return session.GetString("access_token");
        }
    }
}