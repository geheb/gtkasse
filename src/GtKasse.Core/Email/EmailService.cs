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
    private readonly SmtpDispatcher _emailSender;
    private readonly UnitOfWork _unitOfWork;
    private readonly IdentityRepository _identityRepository;

    public EmailService(
        ILogger<EmailService> logger,
        IOptions<ConfirmEmailDataProtectionTokenProviderOptions> confirmEmailOptions,
        IOptions<DataProtectionTokenProviderOptions> changeEmailPassOptions,
        SmtpDispatcher emailSender,
        UnitOfWork unitOfWork,
        IdentityRepository identityRepository)
    {
        _confirmEmailTimeout = confirmEmailOptions.Value.TokenLifespan;
        _changeEmailPassTimeout = changeEmailPassOptions.Value.TokenLifespan;
        _logger = logger;
        _emailSender = emailSender;
        _unitOfWork = unitOfWork;
        _identityRepository = identityRepository;
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

        var dto = new EmailQueueDto
        {
            Recipient = user.Email,
            Subject = model.title,
            HtmlBody = htmlBody,
            IsPrio = true,
        };

        await _unitOfWork.EmailQueue.Create(dto, cancellationToken);
        return await _unitOfWork.Save(cancellationToken) > 0;
    }

    public async Task<bool> EnqueueChangeEmail(IdentityUserGuid user, string newEmail, string callbackUrl, CancellationToken cancellationToken)
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

        var dto = new EmailQueueDto
        {
            Recipient = newEmail,
            Subject = model.title,
            HtmlBody = htmlBody,
            IsPrio = true,
        };

        await _unitOfWork.EmailQueue.Create(dto, cancellationToken);
        return await _unitOfWork.Save(cancellationToken) > 0;
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

        var dto = new EmailQueueDto
        {
            Recipient = user.Email,
            Subject = model.title,
            HtmlBody = htmlBody,
            IsPrio = true,
        };

        await _unitOfWork.EmailQueue.Create(dto, cancellationToken);
        return await _unitOfWork.Save(cancellationToken) > 0;
    }

    public async Task<bool> EnqueMailing(Guid id, CancellationToken cancellationToken)
    {
        var mailing = await _unitOfWork.Mailings.Find(id, cancellationToken);
        if (mailing is null)
        {
            return false;
        }

        var recipients = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (mailing.Value.CanSendToAllMembers)
        {
            var users = await _identityRepository.GetAll(cancellationToken);
            recipients = new(users
                .Where(u => u.IsEmailConfirmed && u.Roles!.Any(r => r == Roles.Member))
                .Select(u => u.Email!), 
                StringComparer.OrdinalIgnoreCase);
        }

        if (mailing.Value.OtherRecipients?.Length > 0)
        {
            foreach(var recipient in mailing.Value.OtherRecipients)
            {
                recipients.Add(recipient);
            }
        }

        await using var trans = await _unitOfWork.BeginTran(cancellationToken);

        foreach (var email in recipients)
        {
            var dto = new EmailQueueDto
            {
                Recipient = email,
                ReplyAddress = mailing.Value.ReplyAddress,
                Subject = mailing.Value.Subject,
                HtmlBody = mailing.Value.Body,
            };
            await _unitOfWork.EmailQueue.Create(dto, cancellationToken);
        }

        var result = await _unitOfWork.Mailings.UpdateClosed(mailing.Value.Id, recipients.Count, cancellationToken);
        if (result.IsFailed)
        {
            return false;
        }

        if (await _unitOfWork.Save(cancellationToken) < 1)
        {
            return false;
        }

        await trans.CommitAsync(cancellationToken);

        return true;
    }

    public async Task HandleEmails(CancellationToken cancellationToken)
    {
        var emailQueue = await _unitOfWork.EmailQueue.GetNextToSend(10, cancellationToken);
        if (emailQueue.Length == 0)
        {
            return;
        }

        var items = emailQueue.Select(e => new EmailItem
        {
            Subject = e.Subject!,
            HtmlBody = e.HtmlBody!,
            Recipient = e.Recipient!,
            ReplyTo = e.ReplyAddress,
        }).ToArray();

        await _emailSender.Send(items, cancellationToken);

        var result = await _unitOfWork.EmailQueue.UpdateSent([.. emailQueue.Select(e => e.Id)], cancellationToken);

        if (result.IsSuccess)
        {
            await _unitOfWork.Save(cancellationToken);
        }
    }
}
