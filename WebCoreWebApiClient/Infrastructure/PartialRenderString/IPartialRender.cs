using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebCoreWebApiClient.Infrastructure.PartialRenderString
{
    public interface IPartialRender
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewName">full viewname => views/(foldername)/viewname</param>
        /// <param name="model">Object model that is to passed to the view for rendering</param>
        /// <returns></returns>
        Task<string> RenderToStringAsync(string viewName, object model);
    }
}
