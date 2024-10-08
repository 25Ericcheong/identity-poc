using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreApi.Controllers;

[ApiController]
[Authorize(Policy = Constants.PolicyName.ApiScope)]
[AllowAnonymous]
[Route("api/[controller]")]
public class CompanyController : ControllerBase
{
    [HttpGet]
    public string Index()
    {
        return $"COMPANY DATA COMING FROM NET CORE API ({Constants.AllowedScopes.CoreApiScope} Protected)";
    }
}