using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.RequestResponse;

public class ErrorRequest
{
    [FromQuery(Name = "errorId")]
    public string? ErrorId { get; set; }
}