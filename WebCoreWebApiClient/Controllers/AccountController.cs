﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebCoreWebApiClient.Models;
using WebCoreWebApiClient.Models.Http.Client;

namespace WebCoreWebApiClient.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly MyTypeWebClient typeClient;

        public AccountController(IHttpClientFactory httpClientFactory, MyTypeWebClient typeClient)
        {
            this.httpClientFactory = httpClientFactory;
            this.typeClient = typeClient;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult Login(string ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
           
           return  View(new UserInfo());
        }


        [HttpPost]
        public async Task<IActionResult> Login(UserInfo user, string returnUrl)
        {
            var client = httpClientFactory.CreateClient("MyWebApiClient");
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            string jsonStringIfyData = JsonConvert.SerializeObject(user);

            var contentData = new StringContent(jsonStringIfyData, System.Text.Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/v2.2/Auth/Login", contentData);

            if(response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var responsecontent = await response.Content.ReadAsStringAsync();
                AuthResponse tokenresponse = JsonConvert.DeserializeObject<AuthResponse>(responsecontent);
                var token = tokenresponse.accessToken;
                var expdate = tokenresponse.expirationDateTime;
                var issuedDate = tokenresponse.dateIssued;
                var refreshtoken = tokenresponse.refreshToken;

                ISession session = HttpContext.Session;

                var userClaims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, user.userName),
                    new Claim(ClaimTypes.Role, "Admin"),
                    new Claim(ClaimTypes.Email,user.userName),
                    new Claim(ClaimTypes.GivenName, user.userName)

                };

                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                identity.AddClaims(userClaims);

                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                session.SetString("responseAuth", responsecontent);
                if(!string.IsNullOrEmpty(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                return RedirectToAction("Index", "Download");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {

                return View("Unauthorized");
            }
            else
            {
                return View("errorDownload");

            }
           
          

        }


        public IActionResult SignalR()
        {
            return View("SignalR");
        }
       
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync();
          
            return RedirectToAction("Login");
        }


        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}