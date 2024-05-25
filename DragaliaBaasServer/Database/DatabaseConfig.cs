using DragaliaBaasServer.Database.Repositories;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace DragaliaBaasServer.Database;

public static class DatabaseConfig
{
    private const int MaxMigrationAttempts = 5;

    public static IServiceCollection ConfigureDbServices(this IServiceCollection services) =>
        services
            .AddDbContext<BaasDbContext>(options =>
            {
                options.UseNpgsql(
                    BuildConnectionString(),
                    x => x.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
                );
            })
            .AddScoped<IAccountRepository, DbAccountRepository>();

    private static string BuildConnectionString()
    {
        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = Environment.GetEnvironmentVariable("BAASDB_HOST") ?? throw new InvalidDataException("Environmental variable BAASDB_HOST not set."),
            Database = Environment.GetEnvironmentVariable("BAASDB_NAME") ?? "baas",
            Username = Environment.GetEnvironmentVariable("BAASDB_USERNAME") ?? throw new InvalidDataException("Environmental variable BAASDB_USERNAME not set."),
            Password = Environment.GetEnvironmentVariable("BAASDB_PASSWORD") ?? throw new InvalidDataException("Environmental variable BAASDB_PASSWORD not set."),
            Port = int.Parse(Environment.GetEnvironmentVariable("BAASDB_PORT") ?? "5432")
        };

        return builder.ConnectionString;
    }

    public static void MigrateDb(this WebApplication app)
    {
        using var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();

        var ctx = scope.ServiceProvider.GetRequiredService<BaasDbContext>();

        var attempts = 0;
        while (!ctx.Database.CanConnect())
        {
            if (attempts++ >= MaxMigrationAttempts)
                throw new InvalidOperationException(
                    $"Failed to apply database migrations: Could not connect to database after {MaxMigrationAttempts} attempts.");

            Thread.Sleep(500);
        }

        if (ctx.Database.GetPendingMigrations().Any())
            ctx.Database.Migrate();
    }
}