using Duende.IdentityServer.Services;
using IdentityServer.RequestResponse;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Controllers;

[AllowAnonymous]
public class HomeController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    /// <summary>
    /// Errors are usually due to misconfiguration and not much user can do. Reference: https://docs.duendesoftware.com/identityserver/v7/ui/error/
    /// </summary>
    /// <param name="request">Has the error id to acquire error message to display to the user</param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> Error(ErrorRequest request)
    {
        var message = await _interactionService.GetErrorContextAsync(request.ErrorId);
        
        if (message != null)
        {
            ErrorResponse errorResponse = new ErrorResponse()
            {
                ErrorCode = message.Error,
                ErrorDescription = message.ErrorDescription,
                ErrorId = request.ErrorId,
            };
            ViewBag.ErrorResponse = errorResponse;
        }
        
        return View();
    }

    public HomeController(IIdentityServerInteractionService interactionService)
    {
        _interactionService = interactionService;
    }
    
    private readonly IIdentityServerInteractionService _interactionService;
}