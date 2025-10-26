using GtKasse.Core.Converter;
using GtKasse.Core.Entities;
using GtKasse.Core.Models;
using GtKasse.Core.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace GtKasse.Core.Email;

public sealed class EmailService
{
    private readonly EmailTemplateRenderer _emailTemplateRenderer = new();
    private readonly TimeSpan _confirmEmailTimeout, _changeEmailPassTimeout;
    private readonly SmtpDispatcher _smtpDispatcher;
    private readonly UnitOfWork _unitOfWork;
    private readonly IdentityRepository _identityRepository;

    public EmailService(
        IOptions<ConfirmEmailDataProtectionTokenProviderOptions> confirmEmailOptions,
        IOptions<DataProtectionTokenProviderOptions> changeEmailPassOptions,
        SmtpDispatcher smtpDispatcher,
        UnitOfWork unitOfWork,
        IdentityRepository identityRepository)
    {
        _confirmEmailTimeout = confirmEmailOptions.Value.TokenLifespan;
        _changeEmailPassTimeout = changeEmailPassOptions.Value.TokenLifespan;
        _smtpDispatcher = smtpDispatcher;
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

        var template = isExtended ? EmailTemplate.ConfirmRegistrationExtended : EmailTemplate.ConfirmRegistration;

        var htmlBody = _emailTemplateRenderer.Render(template, model);

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

        var htmlBody = _emailTemplateRenderer.Render(EmailTemplate.ConfirmChangeEmail, model);

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

        var htmlBody = _emailTemplateRenderer.Render(EmailTemplate.ConfirmPasswordForgotten, model);

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

        var templateModel = new
        {
            title = mailing.Value.Subject,
            body = mailing.Value.Body,
        };

        var htmlBody = _emailTemplateRenderer.Render(EmailTemplate.Mailing, templateModel);

        var users = (await _identityRepository.GetAll(cancellationToken))
            .Where(u => u.IsEmailConfirmed)
            .ToArray();

        var recipients = new Dictionary<string, IdentityDto?>(StringComparer.OrdinalIgnoreCase);
        if (mailing.Value.IsMemberOnly)
        {
            foreach (var u in users.Where(u => u.Roles!.Any(r => r == Roles.Member)))
            {
                recipients.Add(u.Email!, u);
            }
        }
        if (mailing.Value.IsYoungPeople)
        {
            foreach (var u in users.Where(u => u.Mailings?.Any(m => m == UserMailings.YoungPeople) == true))
            {
                recipients.TryAdd(u.Email!, u);
            }
        }

        if (mailing.Value.OtherRecipients?.Length > 0)
        {
            foreach (var recipient in mailing.Value.OtherRecipients)
            {
                var index = Array.FindIndex(users, u => recipient.Equals(u.Email, StringComparison.OrdinalIgnoreCase));
                recipients.TryAdd(recipient, index < 0 ? null : users[index]);
            }
        }

        foreach (var (email, user) in recipients)
        {
            var emailQueue = new EmailQueueDto
            {
                Recipient = email,
                ReplyAddress = mailing.Value.ReplyAddress,
                Subject = mailing.Value.Subject,
                HtmlBody = htmlBody,
            };
            await _unitOfWork.EmailQueue.Create(emailQueue, cancellationToken);

            if (user is not null)
            {
                var myMailing = new MyMailingDto
                {
                    MailingId = mailing.Value.Id,
                    UserId = user.Value.Id
                };
                await _unitOfWork.MyMailings.Create(myMailing, cancellationToken);
            }
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
            ReplyAddress = e.ReplyAddress,
        }).ToArray();

        await _smtpDispatcher.Send(items, cancellationToken);

        var result = await _unitOfWork.EmailQueue.UpdateSent([.. emailQueue.Select(e => e.Id)], cancellationToken);

        if (result.IsSuccess)
        {
            await _unitOfWork.Save(cancellationToken);
        }
    }
}
