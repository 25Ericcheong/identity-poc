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
            
                // allowed URIs to send authorization or authentication tokens to
                RedirectUris = { Constants.Urls.Mvc, Constants.Urls.BffSignInOidc },

                // where to redirect to after logout
                PostLogoutRedirectUris = { Constants.Urls.Mvc, Constants.Urls.BffSignOutOidc },
                
                // create and send refresh token to be used by clients when access token expires without the need to re-login
                AllowOfflineAccess = false,
                
                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    Constants.AllowedScopes.CoreApiScope,
                    Constants.AllowedScopes.FrameworkApiScope,
                }
            } };
}