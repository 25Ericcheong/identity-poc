using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookies";
    options.DefaultChallengeScheme = "oidc"; // used when unauthenticated users log in - begins OpenID connect protocol and redirecting users to auth server
})
    .AddCookie("Cookies") // handler that processes local cookie
    // finally handler that performs OpenID connect protocol
    .AddOpenIdConnect("oidc", options =>
    {
        options.Authority = "https://localhost:5001";
        
        options.ClientId = "web";
        options.ClientSecret = "secret";
        options.ResponseType = "code";

        // since this is true, ASP.NET Core auto stores id, access and refresh tokens in properties of authentication cookie
        options.SaveTokens = true; // to persist tokens in cookie
        
        options.Scope.Clear();
        options.Scope.Add("openid");
        options.Scope.Add("profile");
        
        // to get access to delivery and a refresh token
        options.Scope.Add("delivery");
        options.Scope.Add("offline_access");
        
        // additional claims for verification purposes
        options.Scope.Add("verification");
        options.ClaimActions.MapJsonKey("email_verified", "email_verified");
        
        options.GetClaimsFromUserInfoEndpoint = true; // ensures web client retrieve all claims from userinfo endpoint

        options.MapInboundClaims = false; // Don't rename claim types
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages().RequireAuthorization();

app.Run();
