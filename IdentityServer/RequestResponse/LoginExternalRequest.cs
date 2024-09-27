using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.RequestResponse;

public class LoginExternalRequest
{
    [FromQuery(Name = "ReturnUrl")]
    public string? ReturnUrl { get; set; }
}