using Duende.IdentityServer;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Test;
using IdentityServerStart.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServerStart.Controllers;

public class AccountController : Controller
{
    // GET
    [AllowAnonymous]
    [HttpGet]
    //TODO: Should returnUrl be nullable?
    public IActionResult Login(string? returnUrl)
    {
        if (!string.IsNullOrEmpty(returnUrl))
        {
            ViewBag.ReturnUrl = returnUrl;
        };
        
        return View();
    }
    
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var context = await _interactionService.GetAuthorizationContextAsync(request.ReturnUrl);

        if (ModelState.IsValid)
        {
            if (_userStore.ValidateCredentials(request.Email, request.Password))
            {
                var user = _userStore.FindByUsername(request.Email);
                
                AuthenticationProperties? props = null;
                //TODO: Later on
                // if (LoginOptions.AllowRememberLogin && Input.RememberLogin)
                // {
                //     props = new AuthenticationProperties
                //     {
                //         IsPersistent = true,
                //         ExpiresUtc = DateTimeOffset.UtcNow.Add(LoginOptions.RememberMeLoginDuration)
                //     };
                // };
                
                var identityServerUser = new IdentityServerUser(user.SubjectId)
                {
                    DisplayName = user.Username
                };

                await HttpContext.SignInAsync(identityServerUser, props);

                if (context != null)
                {
                    // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                    // if context is not null, return url is not null
                    return Redirect(request.ReturnUrl!);
                }
                
                // request for a local page
                if (Url.IsLocalUrl(request.ReturnUrl))
                {
                    return Redirect(request.ReturnUrl);
                }

                if (string.IsNullOrEmpty(request.ReturnUrl))
                {
                    return Redirect("~/");
                }

                // user might have clicked on a malicious link - should be logged
                throw new Exception("invalid return URL");
            }
            
            // this sets ModelState.IsValid to false
            ModelState.AddModelError(string.Empty, "Invalid password or email was provided");
        }
        
        // something went wrong, show form error
        if (!string.IsNullOrEmpty(request.ReturnUrl))
        {
            ViewBag.ReturnUrl = request.ReturnUrl;
        }
        return View();
    }
    
    public AccountController(IIdentityServerInteractionService interactionService, TestUserStore userStore)
    {
        _interactionService = interactionService;
        _userStore = userStore;
    }

    private readonly IIdentityServerInteractionService _interactionService;
    private readonly TestUserStore _userStore;
}