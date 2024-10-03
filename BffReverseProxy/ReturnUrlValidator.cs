using Duende.Bff;

namespace BffReverseProxy;

public class ReturnUrlValidator : IReturnUrlValidator
{
    public Task<bool> IsValidAsync(string returnUrl)
    {
        var uri = new Uri(returnUrl);

        const int vueFrontendPort = 5173;
        const int mvcFrameworkPort = 44330;
        int[] validPorts = { vueFrontendPort, mvcFrameworkPort };
        
        if (uri.Host == "localhost" && validPorts.Contains(uri.Port))
        {
            return Task.FromResult(true);
        }
        
        return Task.FromResult(false);
    }
}