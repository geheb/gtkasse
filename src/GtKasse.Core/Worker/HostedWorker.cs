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
        await using var scope = _serviceScopeFactory.CreateAsyncScope();
        var contextInitializer = scope.ServiceProvider.GetRequiredService<AppDbContextInitializer>();
        await contextInitializer.CreateSuperAdmin();
    }

    private async Task HandleEmails(CancellationToken cancellationToken)
    {
        await using var scope = _serviceScopeFactory.CreateAsyncScope();
        var emailService = scope.ServiceProvider.GetRequiredService<EmailService>();

        try
        {
            await emailService.HandleEmails(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error on send emails");
        }
    }

    private async Task MigrateDatabase(CancellationToken cancellationToken)
    {
        await using var scope = _serviceScopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var migrations = await dbContext.Database.GetPendingMigrationsAsync(cancellationToken);
        if (migrations.Any())
        {
            _logger.LogInformation("apply pending migrations '{Migrations}'", string.Join(",", migrations));
            await dbContext.Database.MigrateAsync(cancellationToken);
        }

        var mySql = scope.ServiceProvider.GetRequiredService<MySqlMigration>();
        await mySql.Migrate(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await MigrateDatabase(stoppingToken);

        await HandleSuperUser();

        var rand = new Random(Environment.TickCount);

        while (!stoppingToken.IsCancellationRequested)
        {
            await HandleEmails(stoppingToken);

            var waitMs = rand.Next(20, 30) * 1000;

            await Task.Delay(waitMs, stoppingToken);
        }
    }
}
