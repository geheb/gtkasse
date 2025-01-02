using GtKasse.Core.Database;
using GtKasse.Core.Email;
using GtKasse.Core.Entities;
using GtKasse.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Mail;

namespace GtKasse.Core.Worker;

public sealed class HostedWorker : BackgroundService
{
    private readonly ILogger _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly AccountEmailTemplateRenderer _accountEmailTemplateRenderer = new();
    private readonly IEmailSender _emailSender;

    private readonly TimeSpan _confirmEmailTimeout, _changeEmailPassTimeout;

    public HostedWorker(
        IOptions<ConfirmEmailDataProtectionTokenProviderOptions> confirmEmailOptions,
        IOptions<DataProtectionTokenProviderOptions> changeEmailPassOptions,
        ILogger<HostedWorker> logger,
        IServiceScopeFactory serviceScopeFactory,
        IEmailSender emailSender)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
        _emailSender = emailSender;

        _confirmEmailTimeout = confirmEmailOptions.Value.TokenLifespan;
        _changeEmailPassTimeout = changeEmailPassOptions.Value.TokenLifespan;
    }

    private async Task HandleSuperUser()
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var contextInitializer = scope.ServiceProvider.GetRequiredService<AppDbContextInitializer>();
        await contextInitializer.CreateSuperAdmin();
    }

    private async Task HandleEmails(CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var dbSet = dbContext.Set<AccountNotification>();

        var entities = await dbSet
            .Include(e => e.User)
            .Where(e => e.SentOn == null)
            .Take(32)
            .ToArrayAsync(cancellationToken);

        if (!entities.Any()) return;

        var count = 0;

        foreach (var entity in entities)
        {
            if (await HandleUser(entity, cancellationToken))
            {
                entity.SentOn = DateTimeOffset.UtcNow;
                count++;
            }
        }

        if (count > 0)
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    private static string Format(TimeSpan span) => span.TotalDays > 1 ? $"{span.TotalDays} Tage" : $"{span.TotalHours} Stunden";

    private string GetTimeout(AccountEmailTemplate template)
    {
        switch (template)
        {
            case AccountEmailTemplate.ConfirmRegistration:
            case AccountEmailTemplate.ConfirmRegistrationExtended:
                return Format(_confirmEmailTimeout);
            default:
                return Format(_changeEmailPassTimeout);
        }
    }

    private static string GetTitle(AccountEmailTemplate template)
    {
        return template switch
        {
            AccountEmailTemplate.ConfirmRegistration => "GT Kasse - Registrierung",
            AccountEmailTemplate.ConfirmRegistrationExtended => "GT Kasse - Registrierung",
            AccountEmailTemplate.ConfirmPasswordForgotten => "GT Kasse - Passwort",
            AccountEmailTemplate.ConfirmChangeEmail => "GT Kasse - E-Mail-Adresse",
            _ => throw new NotImplementedException($"unknown {nameof(AccountEmailTemplate)} {template}")
        };
    }

    private async Task<bool> HandleUser(AccountNotification entity, CancellationToken cancellationToken)
    {
        var template = (AccountEmailTemplate)entity.Type;
        if (entity.User == null) throw new InvalidOperationException("user cant't be null");

        var model = new
        {
            title = GetTitle(template),
            name = entity.User.Name!.Split(' ')[0],
            link = entity.CallbackUrl,
            timeout = GetTimeout(template)
        };

        
        var message = await _accountEmailTemplateRenderer.Render(template, model);
        var user = entity.User;

        try
        {
            await _emailSender.SendEmailAsync(user.Email!, model.title, message);
            return true;
        }
        catch (SmtpException ex)
        {
            _logger.LogError(ex, $"send email {template} for user {user.Id} failed");
        }

        return false;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await HandleSuperUser();

        while (!stoppingToken.IsCancellationRequested)
        {
            await HandleEmails(stoppingToken);

            await Task.Delay(30000, stoppingToken);
        }
    }
}
