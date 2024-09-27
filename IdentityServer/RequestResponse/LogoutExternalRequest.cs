using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.RequestResponse;

public class LogoutExternalRequest
{
    [FromQuery(Name = "logoutId")]
    public required string LogoutId { get; set; }
}