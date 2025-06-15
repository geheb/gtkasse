using GtKasse.Core.Converter;
using GtKasse.Core.Entities;
using GtKasse.Core.Models;
using GtKasse.Core.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Mail;

namespace GtKasse.Core.Email;

public sealed class EmailService
{
    private readonly AccountEmailTemplateRenderer _accountEmailTemplateRenderer = new();
    private readonly TimeSpan _confirmEmailTimeout, _changeEmailPassTimeout;
    private readonly ILogger _logger;
    private readonly IEmailSender _emailSender;
    private readonly EmailQueueRepository _repository;

    public EmailService(
        ILogger<EmailService> logger,
        IOptions<ConfirmEmailDataProtectionTokenProviderOptions> confirmEmailOptions,
        IOptions<DataProtectionTokenProviderOptions> changeEmailPassOptions,
        IEmailSender emailSender,
        EmailQueueRepository repository)
    {
        _confirmEmailTimeout = confirmEmailOptions.Value.TokenLifespan;
        _changeEmailPassTimeout = changeEmailPassOptions.Value.TokenLifespan;
        _logger = logger;
        _emailSender = emailSender;
        _repository = repository;
    }

    public async Task<bool> EnqueueConfirmRegistration(IdentityUserGuid user, string callbackUrl, bool isExtended, CancellationToken cancellationToken)
    {
        var dc = new GermanDateTimeConverter();

        var model = new
        {
            title = "GT Kasse - Registrierung",
            name = user.Name!.Split(' ')[0],
            link = callbackUrl,
            timeout = dc.Format(_confirmEmailTimeout)
        };

        var template = isExtended ? AccountEmailTemplate.ConfirmRegistrationExtended : AccountEmailTemplate.ConfirmRegistration;

        var htmlBody = await _accountEmailTemplateRenderer.Render(template, model);

        var entity = new EmailQueueDto
        {
            Recipient = user.Email,
            Subject = model.title,
            HtmlBody = htmlBody,
            IsPrio = true,
        };

        return await _repository.Create(entity, cancellationToken);
    }

    public async Task<bool> EnqueueChangeEmail(IdentityUserGuid user, string callbackUrl, CancellationToken cancellationToken)
    {
        var dc = new GermanDateTimeConverter();

        var model = new
        {
            title = "GT Kasse - E-Mail-Adresse",
            name = user.Name!.Split(' ')[0],
            link = callbackUrl,
            timeout = dc.Format(_changeEmailPassTimeout)
        };

        var htmlBody = await _accountEmailTemplateRenderer.Render(AccountEmailTemplate.ConfirmChangeEmail, model);

        var entity = new EmailQueueDto
        {
            Recipient = user.Email,
            Subject = model.title,
            HtmlBody = htmlBody,
            IsPrio = true,
        };

        return await _repository.Create(entity, cancellationToken);
    }

    public async Task<bool> EnqueueChangePassword(IdentityUserGuid user, string callbackUrl, CancellationToken cancellationToken)
    {
        var dc = new GermanDateTimeConverter();

        var model = new
        {
            title = "GT Kasse - Passwort",
            name = user.Name!.Split(' ')[0],
            link = callbackUrl,
            timeout = dc.Format(_changeEmailPassTimeout)
        };

        var htmlBody = await _accountEmailTemplateRenderer.Render(AccountEmailTemplate.ConfirmPasswordForgotten, model);

        var entity = new EmailQueueDto
        {
            Recipient = user.Email,
            Subject = model.title,
            HtmlBody = htmlBody,
            IsPrio = true,
        };

        return await _repository.Create(entity, cancellationToken);
    }

    public async Task HandleEmails(CancellationToken cancellationToken)
    {
        var items = await _repository.GetNextToSend(50, cancellationToken);
        if (items.Length == 0)
        {
            return;
        }

        var random = new Random(Environment.TickCount);

        foreach (var item in items)
        {
            try
            {
                await _emailSender.SendEmailAsync(item.Recipient!, item.Subject!, item.HtmlBody!);
                if (!await _repository.UpdateSent(item.Id, cancellationToken))
                {
                    _logger.LogError($"update sent email {item.Id} failed");
                }
                else
                {
                    await Task.Delay(random.Next(1000, 3000), cancellationToken);
                }
            }
            catch (SmtpException ex)
            {
                _logger.LogWarning(ex, $"send email {item.Id} failed");
            }
        }
    }
}
