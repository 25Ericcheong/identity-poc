using System.Reflection;
using Duende.IdentityServer.EntityFramework.DbContexts;
using IdentityServer.TestData;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace IdentityServer;

internal static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        // https://learn.microsoft.com/en-us/dotnet/api/microsoft.data.sqlclient.sqlconnection.connectionstring?view=sqlclient-dotnet-standard-5.2#remarks
        const string connectionString = "Server=.;Database=Fathom-Identity; Integrated Security=True; ApplicationIntent=ReadWrite; Encrypt=False";
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
            
            // https://github.com/DuendeSoftware/IdentityServer/blob/main/src/EntityFramework/IdentityServerEntityFrameworkBuilderExtensions.cs#L99
            .AddOperationalStore(options =>
            {
                options.ConfigureDbContext = dbContextOptionsBuilder =>
                    dbContextOptionsBuilder.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
                
                // enables automatic token clean up
                // TODO: To test
                options.EnableTokenCleanup = true;
                
                // 30 seconds to clean token up (for expired tokens)
                options.TokenCleanupInterval = 30;
            });

        return builder.Build();
    }

    private static void InitializeDatabase(IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>()!.CreateScope();
        serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();
    }
    
    public static WebApplication ConfigurePipeline(this WebApplication app)
    { 
        app.UseSerilogRequestLogging();
    
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        // database migration is executed upon running app
        InitializeDatabase(app);
        
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
