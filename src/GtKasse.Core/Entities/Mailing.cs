using GtKasse.Core.Converter;
using GtKasse.Core.Models;

namespace GtKasse.Core.Entities;

public sealed class Mailing : IEntity, IDtoMapper<MailingDto>
{
    public Guid Id { get; set; }
    public DateTimeOffset Created { get; set; }
    public DateTimeOffset? Updated { get; set; }
    public bool CanSendToAllMembers { get; set; }
    public string? OtherRecipients { get; set; }
    public string? ReplyAddress { get; set; }
    public string? Subject { get; set; }
    public string? HtmlBody { get; set; }
    public bool IsClosed { get; set; }
    public int EmailCount { get; set; }
    internal ICollection<MyMailing>? MyMailings { get; set; }

    public void FromDto(MailingDto model)
    {
        Id = model.Id;
        CanSendToAllMembers = model.CanSendToAllMembers;
        OtherRecipients = model.OtherRecipients is not null ? string.Join(',', model.OtherRecipients) : null;
        ReplyAddress = model.ReplyAddress;
        Subject = model.Subject;
        HtmlBody = model.Body;
        IsClosed = model.IsClosed;
        EmailCount = model.EmailCount;
    }

    public MailingDto ToDto(GermanDateTimeConverter dc) => new()
    {
        Id = Id,
        LastUpdate = dc.ToLocal(Updated ?? Created),
        CanSendToAllMembers = CanSendToAllMembers,
        OtherRecipients = OtherRecipients?.Split(','),
        ReplyAddress = ReplyAddress,
        Subject = Subject,
        Body = HtmlBody,
        IsClosed = IsClosed,
        EmailCount = EmailCount,
    };
}
