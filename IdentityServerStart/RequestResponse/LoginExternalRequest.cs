using Microsoft.AspNetCore.Mvc;

namespace IdentityServerStart.RequestResponse;

public class LoginExternalRequest
{
    [FromQuery(Name = "ReturnUrl")]
    public string? ReturnUrl { get; set; }
}