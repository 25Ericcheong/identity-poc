using System;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace FrameworkApi
{
    public class ApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            
            GlobalConfiguration.Configure(config =>
            {
                config.MapHttpAttributeRoutes();
            
                config.Routes.MapHttpRoute(
                    name: "DefaultApi",
                    routeTemplate: "api/{controller}/{id}",
                    defaults: new { id = RouteParameter.Optional }
                );
            });
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            
        }
    }
}