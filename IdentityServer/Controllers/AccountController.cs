using Duende.IdentityServer;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Test;
using IdentityModel;
using IdentityServer.RequestResponse;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoginRequest = IdentityServer.RequestResponse.LoginRequest;

namespace IdentityServer.Controllers;

[AllowAnonymous]
public class AccountController : Controller
{
    /// <summary>
    /// This is where users are redirected to for login and authentication (externally, from applications) 
    /// </summary>
    /// <param name="request">Request from user's application to login that has returnUrl query parameter</param>
    /// <returns></returns>
    [HttpGet]
    //TODO: Can/should returnUrl and logoutId be nullable? Should be no. Would need to redirect users to somewhere after login.
    public IActionResult Login(LoginExternalRequest request)
    {
        if (!string.IsNullOrEmpty(request.ReturnUrl))
        {
            ViewBag.ReturnUrl = request.ReturnUrl;
        };
        
        return View();
    }
    
    /// <summary>
    /// This is where user credentials are validated and if it is wrong the same view will be rendered with the relevant error
    /// </summary>
    /// <param name="request">Request</param>
    /// <returns></returns>
    /// <exception cref="Exception">Occurs when the return url provided is not in an expected format</exception>
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
                
                //TODO: SubjectId to be replaced with GUID data type to uniquely identity user and must never change and must never be reassigned to a different user
                // https://docs.duendesoftware.com/identityserver/v7/ui/login/session/
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
                throw new Exception("Invalid return URL provided");
            }
            
            // this sets ModelState.IsValid to false
            ModelState.AddModelError(string.Empty, "Invalid password or email was provided");
        }
        
        // something went wrong, show same view with error
        if (!string.IsNullOrEmpty(request.ReturnUrl))
        {
            ViewBag.ReturnUrl = request.ReturnUrl;
        }
        return View();
    }

    /// <summary>
    /// Where users are redirected to for logout. The response of this action will provide user a log out prompt to confirm their actions. Can be configured to skip step altogether
    /// </summary>
    /// <param name="request">Request containing logout id to acquire logout context</param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> Logout(LogoutExternalRequest request)
    {
        if (!string.IsNullOrEmpty(request.LogoutId))
        {
            ViewBag.LogoutId = request.LogoutId;
        };
        
        // https://docs.duendesoftware.com/identityserver/v6/ui/logout/session_cleanup/
        // typical to prompt user to logout if not attacker could hotlink to logout page directly 
        var showLogoutPrompt = true;
        if (User.Identity?.IsAuthenticated != true)
        {
            showLogoutPrompt = false;
        }
        else
        {
            var context = await _interactionService.GetLogoutContextAsync(request.LogoutId);
            if (context.ShowSignoutPrompt == false)
            {
                showLogoutPrompt = false;
            }
        }
        
        if (showLogoutPrompt == false)
        {
            return await Logout(new LogoutRequest
            {
                LogoutId = request.LogoutId,
            });
        }
        
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Logout(LogoutRequest request)
    {
        if (User.Identity?.IsAuthenticated == true) 
        {
            // if there's no current logout context, we need to create one
            // this captures necessary info from the current logged in user
            // this can still return null if there is no context needed
            var logoutId = request.LogoutId ?? await _interactionService.CreateLogoutContextAsync();
            
            // delete local authentication cookie
            await HttpContext.SignOutAsync();
            
            // see if we need to trigger federated logout
            var idp = User.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;
            
            // if it's a local login we can ignore this workflow
            if (idp != null && idp != IdentityServerConstants.LocalIdentityProvider)
            {
                // we need to see if the provider supports external logout
                if (await HttpContext.GetSchemeSupportsSignOutAsync(idp))
                {
                    // build a return URL so the upstream provider will redirect back
                    // to us after the user has logged out. this allows us to then
                    // complete our single sign-out processing.
                    var url = Url.Action("LoggedOut", "Account", new LoggedOutRequest
                    {
                        LogoutId = logoutId,
                    });
                    
                    // this triggers a redirect to the external provider for sign-out
                    return SignOut(new AuthenticationProperties { RedirectUri = url }, idp);
                }
            }
        }
        
        return await LoggedOut(new LoggedOutRequest
        {
            LogoutId = request.LogoutId,
        });
    }

    [HttpPost]
    public async Task<IActionResult> LoggedOut(LoggedOutRequest request)
    {
        // get context information (client name, post logout redirect URI and iframe for federated signout)
        var logout = await _interactionService.GetLogoutContextAsync(request.LogoutId);

        // https://docs.duendesoftware.com/identityserver/v6/ui/logout/client_redirect/
        // logout page should not directly redirect user back to client since this may skip front-channel notification (would only be the case if we have multiple applications tied to a single client)
        // if (logout.PostLogoutRedirectUri is not null)
        // {
        //     return Redirect(logout.PostLogoutRedirectUri);
        // }
        
        LoggedOutResponse loggedOutResponse = new LoggedOutResponse() {
            PostLogoutRedirectUri = logout.PostLogoutRedirectUri,
            AutomaticRedirectAfterLogout = false, // can be modified if need be to streamline process
            ClientName = string.IsNullOrEmpty(logout.ClientName) ? logout.ClientId : logout.ClientName,
            SignOutIframeUrl = logout.SignOutIFrameUrl
        };
        ViewBag.LoggedOutResponse = loggedOutResponse;
        
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