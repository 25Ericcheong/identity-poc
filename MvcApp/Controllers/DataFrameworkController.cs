using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using IdentityModel.Client;

namespace MvcApp.Controllers
{
    [Authorize]
    public class DataFrameworkController : Controller
    {
        private static DiscoveryCache _discoveryCache = new DiscoveryCache(Urls.IdentityServer);
        
        public async Task<ActionResult> Index()
        {
            var authResult = await HttpContext.GetOwinContext().Authentication.AuthenticateAsync("cookies");
            
            return View("Index", model: "testing");
        }
    }
}