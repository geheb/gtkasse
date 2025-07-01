using GtKasse.Core.Entities;

namespace GtKasse.Core.Models;

public struct EmailQueueDto : IDto
{
    public Guid Id { get; set; }

    public string? Recipient { get; set; }

    public string? ReplyAddress { get; set; }

    public string? Subject { get; set; }

    public string? HtmlBody { get; set; }

    public bool IsPrio { get; set; }
}
