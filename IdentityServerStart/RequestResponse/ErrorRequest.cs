using Microsoft.AspNetCore.Mvc;

namespace IdentityServerStart.RequestResponse;

public class ErrorRequest
{
    [FromQuery(Name = "errorId")]
    public string? ErrorId { get; set; }
}