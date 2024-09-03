using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace IdentityServer;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        { 
            new IdentityResources.OpenId(), // subject id
            new IdentityResources.Profile(), // first name, last name, etc.
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
            { new ApiScope(name: "delivery", displayName: "Delivery Name") };

    public static IEnumerable<Client> Clients =>
        new Client[] 
            { 
                new Client // for machine-to-machine client
                {
                    ClientId = "client",

                    // no interactive user, use the clientid/secret for authentication
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    // secret for authentication
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },

                    // scopes that client has access to
                    AllowedScopes = { "delivery" }
                },
                new Client // for interactive web app
                {
                    ClientId = "web",

                    AllowedGrantTypes = GrantTypes.Code,

                    // where to redirect after login
                    RedirectUris = { "https://localhost:5002/signin-oidc" },
                    
                    // where to redirect to after logout
                    PostLogoutRedirectUris = { "https://localhost:5002/signout-callback-oidc" },
                    
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile
                    }
                }
            };
}