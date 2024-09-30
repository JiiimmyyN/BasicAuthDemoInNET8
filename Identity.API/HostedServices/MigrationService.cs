using Identity.API.Data;
using Microsoft.EntityFrameworkCore;

namespace Identity.API.HostedServices;

public class MigrationService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public MigrationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<MigrationService>>();

        try
        {
            logger.LogInformation("Migrating databases");

            await MigrateDbContextAsync<ApplicationDbContext>(services);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating the databases");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private static async Task MigrateDbContextAsync<TContext>(IServiceProvider services) where TContext : DbContext
    {
        var context = services.GetRequiredService<TContext>();
        await context.Database.MigrateAsync();
    }
}