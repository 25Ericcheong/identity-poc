using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace IdentityServer;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        { 
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        {
            new ApiScope(name: Constants.AllowedScopes.CoreApiScope, displayName: "Core API Scope"),
            new ApiScope(name: Constants.AllowedScopes.FrameworkApiScope, displayName: "Framework API Scope") 
        };

    public static IEnumerable<Client> Clients =>
        new List<Client> 
            { new Client
            {
                ClientId = "web",
                ClientSecrets = { new Secret("secret".Sha256()) },

                AllowedGrantTypes = GrantTypes.Code,
            
                // where to redirect to after login
                RedirectUris = { Constants.Urls.Mvc },

                // where to redirect to after logout
                PostLogoutRedirectUris = { Constants.Urls.Mvc },

                AllowOfflineAccess = true,
                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    Constants.AllowedScopes.CoreApiScope,
                    Constants.AllowedScopes.FrameworkApiScope,
                }
            } };
}