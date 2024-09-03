using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using IdentityModel;

namespace IdentityServer;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        { 
            new IdentityResources.OpenId(), // subject id
            new IdentityResources.Profile(), // first name, last name, etc. note: this does not include email addresses
            new IdentityResource()
            {
                Name = "verification", // the name is the scope that clients can request to get associated UserClaims
                UserClaims = new List<string> 
                { 
                    JwtClaimTypes.Email,
                    JwtClaimTypes.EmailVerified
                }
            }
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
                    ClientSecrets = { new Secret("secret".Sha256()) },

                    // scopes that client has access to
                    AllowedScopes = { "delivery" }
                },
                new Client // for interactive web app
                {
                    ClientId = "web",
                    ClientSecrets = { new Secret("secret".Sha256()) },

                    AllowedGrantTypes = GrantTypes.Code,

                    // where to redirect after login
                    RedirectUris = { "https://localhost:5002/signin-oidc" },
                    
                    // where to redirect to after logout
                    PostLogoutRedirectUris = { "https://localhost:5002/signout-callback-oidc" },
                    
                    AllowOfflineAccess = true,
                    
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "verification", // previously added another scope to emit additional claims
                        "delivery",
                    }
                },
                new Client // for interactive web app
                {
                    ClientId = "bff",
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    
                    AllowedGrantTypes = GrantTypes.Code,
                    
                    RedirectUris = { "https://localhost:5003/signin-oidc" },
                    PostLogoutRedirectUris = { "https://localhost:5003/signout-callback-oidc" },
                    
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "delivery"
                    }
                }
            };
}