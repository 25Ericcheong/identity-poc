namespace IdentityServerStart.Pages.Account.Login;

public class ViewModel
{
    public bool AllowRememberLogin { get; set; } = true;
    public bool EnableLocalLogin { get; set; } = true;
    
    public IEnumerable<ExternalProvider> ExternalProviders { get; set; } = Enumerable.Empty<ExternalProvider>();
    
    public IEnumerable<ExternalProvider> ExternalProvidersWithDisplayName => ExternalProviders.Where(x => !String.IsNullOrWhiteSpace(x.DisplayName));
    
    public class ExternalProvider(string authenticationScheme, string? displayName = null)
    {
        public string? DisplayName { get; set; } = displayName;
        public string AuthenticationScheme { get; set; } = authenticationScheme;
    }
}