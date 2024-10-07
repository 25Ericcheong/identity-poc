using System.Net.Http.Headers;
using BffReverseProxy;
using Duende.Bff;
using Duende.Bff.Yarp;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Yarp.ReverseProxy.Transforms;
using Constants = BffReverseProxy.Constants;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddBff();
builder.Services.AddTransient<IReturnUrlValidator, ReturnUrlValidator>();

var reverseProxyConfig = builder.Configuration.GetSection("ReverseProxy") ?? throw new ArgumentException("ReverseProxy configuration is missing. Check your appsettings.json file!");

// transforms incoming request to add access token as bearer token to forward them to allowed apis listed in configurations
builder.Services.AddReverseProxy()
    .LoadFromConfig(reverseProxyConfig)
    .AddTransforms(transformBuilder =>
    {
        transformBuilder.AddRequestTransform(async transformContext =>
        {
            // looks at current http context and looks at authentication's properties for a key "access_token"
            // either access token doesn't exist or is invalid
            // if it is invalid - refresh token will be used to recreate access token
            // possibly a better alternative via Duende: https://docs.duendesoftware.com/identityserver/v7/quickstarts/3a_token_management/#:~:text=An%20object%20called%20tokenInfo%20containing%20all%20stored%20tokens,automatically%20refreshed%20using%20the%20refresh%20token%20if%20needed.
            // but would like to be more aware of how/where tokens are being created
            var accessToken = await transformContext.HttpContext.GetUserAccessTokenAsync();

            if (accessToken.AccessToken != null)
            {
                transformContext.ProxyRequest.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", accessToken.AccessToken);
            }
        });
    }).AddBffExtensions();

builder.Services.AddCors(corOptions =>
{
    corOptions.AddDefaultPolicy(policy =>
    {
        policy
            .WithOrigins(Constants.Urls.VueSpaFrontend)
            .WithHeaders("x-csrf", "content-type")
            .AllowCredentials();
    });
});

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = "cookies";
        options.DefaultChallengeScheme = "oidc";
        options.DefaultSignOutScheme = "oidc";
    })
    .AddCookie("cookies", options =>
    {
        // options.Cookie.Name = "bff_cookies";
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Cookie.HttpOnly = true; // prevent client side from acquiring claims I suppose, to be discussed
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    })
    .AddOpenIdConnect("oidc", options =>
    {
        options.Authority = Constants.Urls.IdentityServer; // if user has no auth session or has invalid auth - request will be "challenge" and forwarded to identity server
        
        options.ResponseType = "code";
        options.ResponseMode = "query";
        
        options.ClientId = "web";
        options.ClientSecret = "secret";
        options.ResponseType = "code";
        options.GetClaimsFromUserInfoEndpoint = true;
        options.MapInboundClaims = false;
        
        options.Scope.Clear();
        // required to hit identity server; if needed
        options.Scope.Add("openid");
        options.Scope.Add("profile");
        
        // required to hit core api endpoints
        options.Scope.Add("CoreApiScope");
        
        // attaches refresh token to auth properties to be used if needed
        options.SaveTokens = true;
        options.UsePkce = true;

        options.TokenValidationParameters = new()
        {
            NameClaimType = "name",
            RoleClaimType = "role"
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAuthenticatedUserPolicy", policy =>
    {
        policy.RequireAuthenticatedUser();
    });
});

var app = builder.Build();

// Enable exception handler middleware in pipeline
app.UseExceptionHandler("/?error");

// Enable HSTS middleware in pipeline
// if (!app.Environment.IsDevelopment())
// {
//     // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//     app.UseHsts();
// }

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

// adds antiforgery protection for local APIs
app.UseBff();

app.MapReverseProxy(proxyApp =>
{
    proxyApp.UseAntiforgeryCheck();
});

// Enable BFF login, logout and userinfo endpoints
app.MapBffManagementEndpoints();

app.MapFallbackToFile("index.html");

app.Run();
