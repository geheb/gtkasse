namespace GtKasse.Core.Entities;

internal sealed class EmailQueue
{
    public Guid Id { get; set; }

    public DateTimeOffset Created { get; set; }

    public DateTimeOffset? Sent { get; set; }

    public string? Recipient { get; set; }

    public string? Subject { get; set; }

    public string? HtmlBody { get; set; }

    public bool IsPrio { get; set; }
}
