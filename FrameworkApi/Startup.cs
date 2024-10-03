using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web.Http;
using FrameworkApi;
using IdentityModel.Client;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Jwt;
using Owin;

[assembly: OwinStartup(typeof(Startup))]

namespace FrameworkApi
{
    public class Startup
    {
        private static readonly DiscoveryCache DiscoveryCache = new(Constants.Urls.IdentityServer);
        
        public void Configuration(IAppBuilder app)
        {
            IdentityModelEventSource.ShowPII = true; 
            
            app.UseJwtBearerAuthentication(new JwtBearerAuthenticationOptions
            {
                AuthenticationType = "jwt",
                AuthenticationMode = AuthenticationMode.Active,
                TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = Constants.Urls.IdentityServer,
                    ValidAudience = Constants.Urls.IdentityServer + "/resources",
                    IssuerSigningKeyResolver = LoadKeys,
                    NameClaimType = "name",
                    RoleClaimType = "role"
                },
            });
            
            HttpConfiguration config = new HttpConfiguration();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            app.UseWebApi(config);
        }
        
        private IEnumerable<SecurityKey> LoadKeys(string token, SecurityToken securityToken, string kid, TokenValidationParameters validationParameters)
        {
            var disco = DiscoveryCache.GetAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            if (disco.IsError) { throw new Exception("Failed to retrieve discovery information - " + disco.Error); }
            
            if (disco.KeySet is null) { throw new Exception("Discovery information's key set should not be null"); }

            var keys = disco.KeySet.Keys
                .Where(x => x.N != null && x.E != null)
                .Select(x => {
                    var rsa = new RSAParameters
                    {
                        Exponent = Base64UrlEncoder.DecodeBytes(x.E),
                        Modulus = Base64UrlEncoder.DecodeBytes(x.N),
                    };

                    return new RsaSecurityKey(rsa)
                    {
                        KeyId = x.Kid
                    };
                });

            return keys;
        }
    }
}