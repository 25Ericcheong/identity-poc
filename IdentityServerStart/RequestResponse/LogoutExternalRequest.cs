using Microsoft.AspNetCore.Mvc;

namespace IdentityServerStart.RequestResponse;

public class LogoutExternalRequest
{
    [FromQuery(Name = "logoutId")]
    public required string LogoutId { get; set; }
}