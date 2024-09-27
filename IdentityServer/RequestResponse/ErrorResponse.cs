namespace IdentityServer.RequestResponse;

public class ErrorResponse
{
    public string? ErrorCode { get; set; }
    
    public string? ErrorDescription { get; set; }
    
    public string? ErrorId { get; set; }
}