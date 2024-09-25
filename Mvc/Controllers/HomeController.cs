using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Mvc.Controllers
{
    public class HomeController : Controller
    {
        public async Task<ActionResult> Index()
        {
            var result = await HttpContext.GetOwinContext().Authentication.AuthenticateAsync("cookies");

            // result can be null if authentication fail or use is not authenticated at all
            if (result is null)
            {
                ViewBag.IsLoggedIn = false;
            }
            else
            {
                ViewBag.IsLoggedIn = result.Identity.IsAuthenticated;
            }
            return View();
        }

        public ActionResult ImportData()
        {
            ViewBag.Message = "Data imported successfully";
            return View();
        }
        
        [Authorize]
        public async Task<ActionResult> ImportDataFramework()
        {
            var result = await HttpContext.GetOwinContext().Authentication.AuthenticateAsync("cookies");
            ViewBag.Message = "Data imported successfully via .NET Framework";
            return View(result);
        }

        public ActionResult Vue()
        {
            ViewBag.Message = "In Vue currently";
            return View();
        }

        public ActionResult Forbidden()
        {
            ViewBag.Message = "You are trying to access a forbidden page";
            return View();
        }

        public void Logout()
        {
            HttpContext.GetOwinContext().Authentication.SignOut("oidc", "cookies");
        }
    }
}