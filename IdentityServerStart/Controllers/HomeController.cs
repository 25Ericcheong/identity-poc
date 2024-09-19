using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServerStart.Controllers;

[AllowAnonymous]
public class HomeController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}