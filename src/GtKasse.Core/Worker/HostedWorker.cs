using GtKasse.Core.Database;
using GtKasse.Core.Email;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GtKasse.Core.Worker;

public sealed class HostedWorker : BackgroundService
{
    private readonly ILogger _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public HostedWorker(
        ILogger<HostedWorker> logger,
        IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    private async Task HandleSuperUser()
    {
        using var scope = _serviceScopeFactory.CreateAsyncScope();
        var contextInitializer = scope.ServiceProvider.GetRequiredService<AppDbContextInitializer>();
        await contextInitializer.CreateSuperAdmin();
    }

    private async Task HandleEmails(CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateAsyncScope();
        var emailService = scope.ServiceProvider.GetRequiredService<EmailService>();

        await emailService.HandleEmails(cancellationToken);
    }

    private async Task MigrateDatabase(CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var migrations = await dbContext.Database.GetPendingMigrationsAsync(cancellationToken);
        if (migrations.Any())
        {
            _logger.LogInformation("apply pending migrations '{Migrations}'", string.Join(",", migrations));
            await dbContext.Database.MigrateAsync(cancellationToken);
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await MigrateDatabase(stoppingToken);

        await HandleSuperUser();

        while (!stoppingToken.IsCancellationRequested)
        {
            await HandleEmails(stoppingToken);

            await Task.Delay(30000, stoppingToken);
        }
    }
}
