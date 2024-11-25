using IdentityServer;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting up");

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((ctx, lc) => lc
        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}")
        .Enrich.FromLogContext()
        .ReadFrom.Configuration(ctx.Configuration));

    builder.Services.AddOpenTelemetry()
        .WithTracing(b =>
        {
            // add jaeger for http tracing across multiple services
            b.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(builder.Environment.ApplicationName))
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddOtlpExporter(opts => { opts.Endpoint = new Uri("http://localhost:4317"); });
        });
    
    var app = builder
        .ConfigureServices()
        .ConfigurePipeline();
    
    app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}");
    
    app.Run();
}
// https://docs.duendesoftware.com/identityserver/v7/quickstarts/4_ef/#handle-expected-exception
catch (Exception ex) when (ex.GetType().Name is not "StopTheHostException")
{
    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    Log.Information("Shut down complete");
    Log.CloseAndFlush();
}