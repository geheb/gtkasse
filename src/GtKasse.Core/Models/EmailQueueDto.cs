namespace GtKasse.Core.Models;

public sealed class EmailQueueDto
{
    public Guid Id { get; set; }

    public string? Recipient { get; set; }

    public string? Subject { get; set; }

    public string? HtmlBody { get; set; }

    public bool IsPrio { get; set; }
}
