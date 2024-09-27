using System.ComponentModel.DataAnnotations;

namespace IdentityServer.RequestResponse;

public class LoginRequest
{
    [Required]
    public required string Email { get; set; }
        
    [Required]
    public required string Password { get; set; }
    
    public string? ReturnUrl { get; set; }
}