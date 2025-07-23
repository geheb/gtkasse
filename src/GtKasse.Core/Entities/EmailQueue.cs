using GtKasse.Core.Converter;
using GtKasse.Core.Models;

namespace GtKasse.Core.Entities;

public sealed class EmailQueue : IEntity, IDtoMapper<EmailQueueDto>
{
    public Guid Id { get; set; }

    public DateTimeOffset Created { get; set; }

    public DateTimeOffset? Updated { get; set; }

    public DateTimeOffset? Sent { get; set; }

    public string? Recipient { get; set; }

    public string? Subject { get; set; }

    public string? HtmlBody { get; set; }

    public bool IsPrio { get; set; }

    public string? ReplyAddress { get; set; }

    public void FromDto(EmailQueueDto model)
    {
        Id = model.Id;
        Recipient = model.Recipient;
        Subject = model.Subject;
        HtmlBody = model.HtmlBody;
        IsPrio = model.IsPrio;
        ReplyAddress = model.ReplyAddress;
    }

    public EmailQueueDto ToDto(GermanDateTimeConverter dc) => new()
    {
        Id = Id,
        Recipient = Recipient,
        Subject = Subject,
        HtmlBody = HtmlBody,
        IsPrio = IsPrio,
        ReplyAddress = ReplyAddress
    };
}
