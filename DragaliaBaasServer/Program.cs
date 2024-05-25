using DragaliaBaasServer.Database;
using DragaliaBaasServer.Middleware;
using DragaliaBaasServer.Models.Backend;
using DragaliaBaasServer.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Identity;
using Serilog;
using Serilog.Events;

namespace DragaliaBaasServer;

public class Program
{
    public static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateBootstrapLogger();

        var builder = WebApplication.CreateBuilder(args);
            
        builder.WebHost.UseStaticWebAssets();

        builder.Logging.ClearProviders();
        builder.Logging.AddSerilog();
        builder.Host.UseSerilog(
            (context, services, loggerConfig) =>
                loggerConfig
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext()
                    .Filter.ByExcluding(c =>
                    {
                        return
                            c.Properties.Any(p => p.Value.ToString().Contains("/_blazor"))
                            && c.Properties.Any(p => p.Value.ToString().Contains("RequestLoggingMiddleware"));
                    })
                    .WriteTo.Console()
                    .WriteTo.File("logs/baas-log-.txt", rollingInterval: RollingInterval.Day)
        );

        builder.Services
            .AddRazorComponents()
            .AddInteractiveServerComponents();
        
        builder.Services.AddCascadingAuthenticationState();

        // Add services to the container.
        builder.Services.ConfigureDbServices();
        builder.Services.AddScoped<IAccountService, AccountService>();
        builder.Services.AddSingleton<ISavefileService, SavefileService>();
        builder.Services.AddScoped<IPasswordHasher<IHasId>, PasswordHasher<IHasId>>();
        builder.Services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();
        builder.Services.AddScoped<IHostEnvironmentAuthenticationStateProvider>(sp => (ServerAuthenticationStateProvider)sp.GetRequiredService<AuthenticationStateProvider>());

        builder.Services.AddSingleton<IAuthorizationService, AuthorizationService>();

        builder.Services.AddHttpClient();
        builder.Services.AddScoped<PatreonService>();

        builder.Services.AddControllers();

        var app = builder.Build();

        if (app.Environment.IsDevelopment() || true)
            app.MigrateDb();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error", createScopeForErrors: true);
        }
        
        using var scope = app.Services.CreateScope();
        var dbCtx = scope.ServiceProvider.GetRequiredService<BaasDbContext>();
        Log.Information("Database information: {webUserCount} Web Accounts, {userCount} Users, {deviceCount} Devices", dbCtx.WebUsers.Count(), dbCtx.Users.Count(), dbCtx.Devices.Count());

        app.UseStaticFiles();
        app.UseSerilogRequestLogging();

        app.UseMiddleware<ErrorLoggingMiddleware>();

        app.UseAuthorization();
        app.UseAuthentication();

        app.UseRouting();
        app.UseAntiforgery();

        app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

        app.MapControllers();

        app.Run();
    }
}