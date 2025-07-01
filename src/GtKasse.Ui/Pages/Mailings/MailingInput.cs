using GtKasse.Core.Email;
using GtKasse.Ui.Annotations;
using System.ComponentModel.DataAnnotations;

namespace GtKasse.Ui.Pages.Mailings;

public sealed class MailingInput
{
    [Display(Name = "Antwortadresse")]
    [EmailField, EmailLengthField]
    public string? ReplyAddress { get; set; }

    [Display(Name = "An alle Mitglieder")]
    public bool CanSendToAllMembers { get; set; }

    [Display(Name = "Weitere Empfänger (Komma getrennt)")]
    [TextLengthField(MinimumLength = 6)]
    public string? OtherRecipients { get; set; }

    [Display(Name = "Betreff")]
    [RequiredField, TextLengthField]
    public string? Subject { get; set; }

    [Display(Name = "Inhalt")]
    [RequiredField]
    public string? Body { get; set; }

    internal void From(MailingDto dto)
    {
        ReplyAddress = dto.ReplyAddress;
        CanSendToAllMembers = dto.CanSendToAllMembers;
        OtherRecipients = dto.OtherRecipients is not null ? string.Join(',', dto.OtherRecipients) : null;
        Subject = dto.Subject;
        Body = dto.Body;
    }

    internal MailingDto ToDto(Guid id = default) => new()
    {
        Id = id,
        ReplyAddress = ReplyAddress,
        CanSendToAllMembers = CanSendToAllMembers,
        OtherRecipients = OtherRecipients?.Split(',', StringSplitOptions.RemoveEmptyEntries),
        Subject = Subject,
        Body = Body
    };

    internal async Task<string[]> Validate(EmailValidatorService emailValidator, CancellationToken cancellationToken)
    {
        var result = new List<string>();

        var emailAttr = new EmailAddressAttribute();

        if (!string.IsNullOrWhiteSpace(ReplyAddress))
        {
            if (!emailAttr.IsValid(ReplyAddress))
            {
                result.Add($"Die Antwort-Adresse {ReplyAddress} ist fehlerhaft.");
            }

            if (!await emailValidator.Validate(ReplyAddress, cancellationToken))
            {
                result.Add($"Die Antwort-Adresse {ReplyAddress} ist ungültig.");
            }
        }

        if (string.IsNullOrWhiteSpace(OtherRecipients) && !CanSendToAllMembers)
        {
            result.Add("Keine Empfänger angegeben.");
        }

        if (!string.IsNullOrWhiteSpace(OtherRecipients))
        {
            foreach (var recipient in OtherRecipients.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                if (!emailAttr.IsValid(recipient))
                {
                    result.Add($"Die Empfänger-Adresse {recipient} ist fehlerhaft.");
                    continue;
                }

                if (!await emailValidator.Validate(recipient, cancellationToken))
                {
                    result.Add($"Die Empfänger-Adresse {recipient} ist ungültig.");
                }
            }
        }

        return [.. result];
    }
}
