using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebCoreWebApiClient.CustomAttributes
{
    public class AuthorizationActionFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ISession session = context.HttpContext.Session;

            Controller controller = context.Controller as Controller;
            if (controller != null && session != null)
            {
                if (string.IsNullOrEmpty(session.GetString("responseAuth")))
                {
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Account" }, { "action", "Login" } });

                    context.HttpContext.Items["Login"] = "false";
                }
                else
                {
                    context.HttpContext.Items["Login"] = "true";
                }

            }
            else
            {
                context.HttpContext.Items["Login"] = null;
            }
            base.OnActionExecuting(context);
        }
    }
}
