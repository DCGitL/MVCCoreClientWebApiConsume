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
            if(controller != null && session != null)
            {
                if (string.IsNullOrEmpty(session.GetString("access_token")))
                {
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary { {"controller","Login" },{"action","Index" } });
                }

            }
            base.OnActionExecuting(context);
        }
    }
}
