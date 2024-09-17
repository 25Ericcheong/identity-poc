using System;
using System.Collections.Generic;
using System.Linq;
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

        public ActionResult Vue()
        {
            ViewBag.Message = "To Vue page";
            return View();
        }
    }
}