using System;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.Owin;
using Microsoft.Owin.Extensions;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Notifications;
using Microsoft.Owin.Security.OpenIdConnect;
using Mvc;
using Owin;

[assembly: OwinStartup(typeof(Startup))]

namespace Mvc
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            Debug.WriteLine("Should be call");
            
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "cookies",
                ExpireTimeSpan = TimeSpan.FromMinutes(10),
                SlidingExpiration = true,
            });
            
            JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                AuthenticationType = "oidc",
                SignInAsAuthenticationType = "cookies",

                Authority = "https://localhost:4999",

                ClientId = "web",
                ClientSecret = "secret",

                RedirectUri = "https://localhost:44344/",
                PostLogoutRedirectUri = "https://localhost:44344/",

                ResponseType = "code",
                Scope = "openid profile",

                UseTokenLifetime = false,
                SaveTokens = true,
                RedeemCode = true,
                UsePkce = true,
                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    RedirectToIdentityProvider = OnRedirectToIdentityProvider,
                }
            });
            
            app.UseStageMarker(PipelineStage.Authenticate);
        }

        private async Task OnRedirectToIdentityProvider(
            RedirectToIdentityProviderNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions>
                notification)
        {
            await SetIdTokenHintOnLogout(notification);
            ForbidInsteadOfChallengeIfAuthenticated(notification);
        }
        
        /// <summary>
        /// Set the id_token_hint parameter during logout so that IdentityServer can safely redirect back here after
        /// logout. Unlike .NET Core authentication handler, the Owin middleware doesn't do this automatically.
        /// </summary>
        /// <param name="notification"></param>
        private async Task SetIdTokenHintOnLogout(
            RedirectToIdentityProviderNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions>
                notification)
        {
            if (notification.ProtocolMessage.PostLogoutRedirectUri != null)
            {
                var auth = await notification.OwinContext.Authentication.AuthenticateAsync("cookies");
                if (auth.Properties.Dictionary.TryGetValue("id_token", out var idToken))
                {
                    notification.ProtocolMessage.IdTokenHint = idToken;
                }
            }
        }

        /// <summary>
        /// Do not challenge if the user is already authenticated, otherwise you get an infinite loop on authorization failure
        /// </summary>
        /// <param name="notification"></param>
        private void ForbidInsteadOfChallengeIfAuthenticated(
            RedirectToIdentityProviderNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions>
                notification)
        {
            if (notification.ProtocolMessage.RequestType == OpenIdConnectRequestType.Authentication &&
                notification.OwinContext.Authentication.User.Identity.IsAuthenticated)
            {
                notification.HandleResponse();
                notification.OwinContext.Response.Redirect("/");
            }
        }
    }
}