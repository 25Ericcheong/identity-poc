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
            var result = await HttpContext.GetOwinContext().Authentication.AuthenticateAsync("cookies");
            var response = new HomeIndexResponse
            {
                IsAuthenticated = false,
            };

            if (result != null)
            {
                response.IsAuthenticated = result.Identity.IsAuthenticated;
            }
            
            return View(response);
        }
    }
}