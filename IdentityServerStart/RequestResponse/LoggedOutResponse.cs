namespace IdentityServerStart.RequestResponse;

public class LoggedOutResponse
{
    public bool AutomaticRedirectAfterLogout { get; set; }
    
    public string? PostLogoutRedirectUri { get; set; }
    
    public string? ClientName { get; set; }
    
    public string? SignOutIframeUrl { get; set; }
}