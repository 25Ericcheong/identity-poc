using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Mvc.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
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
    }
}