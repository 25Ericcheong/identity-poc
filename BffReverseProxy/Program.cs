using System.Net.Http.Headers;
using BffReverseProxy;
using Duende.Bff;
using Duende.Bff.Yarp;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Yarp.ReverseProxy.Transforms;
using Constants = BffReverseProxy.Constants;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddBff(options =>
{
    // this is by default but want to be explicit
    options.ManagementBasePath = "/bff";
});
builder.Services.AddTransient<IReturnUrlValidator, ReturnUrlValidator>();

var reverseProxyConfig = builder.Configuration.GetSection("ReverseProxy") ?? throw new ArgumentException("ReverseProxy configuration is missing. Check your appsettings.json file!");

// proxies requests based on routing rules
builder.Services.AddReverseProxy()
    .AddTransforms<AccessTokenTransformProvider>()
    .LoadFromConfig(reverseProxyConfig)
    
    // transforms incoming request to add access token as bearer token to forward them to allowed apis listed in configurations
    .AddBffExtensions();

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
        options.Cookie.HttpOnly = true;
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
        
        // attaches refresh token to auth properties to be used if needed
        options.SaveTokens = true;
        options.UsePkce = true;
        
        options.Scope.Clear();
        // required to hit identity server; if needed
        options.Scope.Add("openid");
        options.Scope.Add("profile");
        
        // required to hit core api endpoints
        options.Scope.Add("CoreApiScope");


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
        policy
            .RequireAuthenticatedUser()
            .Build();
    });
});

builder.Services.AddOpenTelemetry()
    .WithTracing(b =>
    {
        b.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(builder.Environment.ApplicationName))
            .AddAspNetCoreInstrumentation()
            .AddOtlpExporter(opts => { opts.Endpoint = new Uri("http://localhost:4317"); });
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

// order matters - after routing before authorization
// adds antiforgery protection for local APIs
app.UseBff();

app.UseAuthorization();

// app.MapReverseProxy(proxyApp =>
// {
//     proxyApp.UseAntiforgeryCheck();
// });

// Enable BFF login, logout and userinfo endpoints
app.MapBffManagementEndpoints();

// adds YARP with anti-forgery protection (looks for CRSF token)
app.MapBffReverseProxy();

app.MapFallbackToFile("index.html");

app.Run();
