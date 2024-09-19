using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServerStart.Controllers;

public class AccountController : Controller
{
    // GET
    [AllowAnonymous]
    public IActionResult Login()
    {
        return View();
    }
    
    
}