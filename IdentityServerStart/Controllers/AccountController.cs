using IdentityServerStart.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServerStart.Controllers;

public class AccountController : Controller
{
    // GET
    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login(string returnUrl)
    {
        ViewData["returnUrl"] = returnUrl;
        
        return View();
    }
    
    [AllowAnonymous]
    [HttpPost]
    public IActionResult Login(LoginRequest request)
    {
        return View();
    }
}