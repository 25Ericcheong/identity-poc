using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using MvcApp.RequestResponse;

namespace MvcApp.Controllers
{
    public class HomeController : Controller
    {
        public async Task<ActionResult> Index()
        {
            var authResult = await HttpContext.GetOwinContext().Authentication.AuthenticateAsync("cookies");
            var response = new HomeIndexResponse
            {
                IsAuthenticated = false,
            };

            if (authResult != null)
            {
                response.IsAuthenticated = authResult.Identity.IsAuthenticated;
            }
            
            return View(response);
        }
        
        public void Logout()
        {
            HttpContext.GetOwinContext().Authentication.SignOut("oidc", "cookies");
        }
    }
}