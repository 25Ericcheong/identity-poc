using System.Reflection;
using IdentityServer.TestData;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace IdentityServer;

internal static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        const string connectionString = "Server=localhost;Database=Fathom-Identity;Username=sa;Password=sa;";
        var migrationsAssembly = typeof(Program).GetTypeInfo().Assembly.GetName().Name;
        
        // uncomment if you want to add a UI
        builder.Services.AddRazorPages();

        builder.Services.AddIdentityServer(options =>
            {
                options.UserInteraction.LoginUrl = "/Account/Login";
                options.UserInteraction.LogoutUrl = "/Account/Logout";
                options.UserInteraction.ErrorUrl = "/Home/Error";

                // https://docs.duendesoftware.com/identityserver/v6/fundamentals/resources/api_scopes#authorization-based-on-scopes
                options.EmitStaticAudienceClaim = true;

                options.Authentication.CookieLifetime = TimeSpan.FromSeconds(30);

                // user will have to re-login regardless if they were actively using the application
                options.Authentication.CookieSlidingExpiration = false;
            })
            .AddInMemoryIdentityResources(Config.IdentityResources)
            .AddInMemoryApiScopes(Config.ApiScopes)
            .AddInMemoryClients(Config.Clients)
            .AddTestUsers(TestUsers.Users)
            .AddOperationalStore(options =>
            {
                options.ConfigureDbContext = dbContextOptionsBuilder =>
                    dbContextOptionsBuilder.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
                
                // enables automatic token clean up
                // TODO: To test
                options.EnableTokenCleanup = true;
                options.TokenCleanupInterval = 30;
            });

        return builder.Build();
    }
    
    public static WebApplication ConfigurePipeline(this WebApplication app)
    { 
        app.UseSerilogRequestLogging();
    
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        // uncomment if you want to add a UI
        //app.UseStaticFiles();
        //app.UseRouting();
            
        app.UseIdentityServer();

        // uncomment if you want to add a UI
        app.UseAuthorization();
        app.MapRazorPages().RequireAuthorization();

        return app;
    }
}
