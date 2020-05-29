using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using WebCoreWebApiClient.Models;
using WebCoreWebApiClient.Models.Http.Client;

namespace WebCoreWebApiClient.Infrastructure
{
    public class BaseController : Controller
    {

        

        public async Task<string> GetTokenAsync(MyTypeWebClient typeClient, IHttpContextAccessor httpContextAccessor)
        {
            var sessionAuthResponse = GetSessionAuthResponse(httpContextAccessor);
            if(sessionAuthResponse != null)
            {
                DateTime currentUtcTime = DateTime.UtcNow;
                if(sessionAuthResponse.expirationDateTime < currentUtcTime) //token expired
                {
                    var refreshtoken = new RequestRefreshToken
                    {
                        token = sessionAuthResponse.accessToken,
                        refreshToken = sessionAuthResponse.refreshToken
                    };

                    var authToken = await GetRefreshToken(typeClient, httpContextAccessor, refreshtoken);

                    if(authToken != null)
                    {
                        return authToken.accessToken;
                    }
                }

                return sessionAuthResponse.accessToken;

               
            }

        
          return null;

        }



        private async Task<AuthResponse> GetRefreshToken(MyTypeWebClient typeClient, IHttpContextAccessor httpContextAccessor, RequestRefreshToken requestRefreshToken)
        {
            string jsonStringIfyData = JsonConvert.SerializeObject(requestRefreshToken);
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            typeClient.Client.DefaultRequestHeaders.Accept.Add(contentType);

            var contentData = new StringContent(jsonStringIfyData, System.Text.Encoding.UTF8, "application/json");

            var response = await typeClient.Client.PostAsync("/api/v2.2/Auth/RefreshToken", contentData);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {

                var responsecontent = await response.Content.ReadAsStringAsync();
                var refreshtokenresult = JsonConvert.DeserializeObject<RefreshTokenResponse>(responsecontent);
                if (refreshtokenresult.success)
                {
                   var  tokenresponse = new AuthResponse
                    {
                        accessToken = refreshtokenresult.token,
                        refreshToken = refreshtokenresult.refreshToken,
                        expirationDateTime = refreshtokenresult.accessTokenExpiration,
                        dateIssued = refreshtokenresult.issuedDate
                    };
                    var responseAuth = JsonConvert.SerializeObject(tokenresponse);

                    httpContextAccessor.HttpContext.Session.SetString("responseAuth", responseAuth);

                    return tokenresponse;
                }

            }

            return null;


        }

        private AuthResponse GetSessionAuthResponse(IHttpContextAccessor httpContextAccessor)
        {
            var returnval = httpContextAccessor.HttpContext.Session.GetString("responseAuth");

            if (!string.IsNullOrEmpty(returnval))
            {
                var tokenval = JsonConvert.DeserializeObject<AuthResponse>(returnval);
             
                ViewData["Login"] = httpContextAccessor.HttpContext.Items["Login"];
                return tokenval;
            }
            return null;
        }
    }
}
