using GtKasse.Core.Converter;
using GtKasse.Core.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GtKasse.Core.Entities;

[Table("email_queue")]
public sealed class EmailQueue : IEntity, IDtoMapper<EmailQueueDto>
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; }

    [Required]
    public DateTimeOffset Created { get; set; }

    public DateTimeOffset? Updated { get; set; }

    public DateTimeOffset? Sent { get; set; }

    [Required]
    public string? Recipient { get; set; }

    [Required]
    public string? Subject { get; set; }

    [Required]
    public string? HtmlBody { get; set; }

    public bool IsPrio { get; set; }

    public void FromDto(EmailQueueDto model)
    {
        Id = model.Id;
        Recipient = model.Recipient;
        Subject = model.Subject;
        HtmlBody = model.HtmlBody;
        IsPrio = model.IsPrio;
    }

    public EmailQueueDto ToDto(GermanDateTimeConverter dc) => new()
    {
        Id = Id,
        Recipient = Recipient,
        Subject = Subject,
        HtmlBody = HtmlBody,
        IsPrio = IsPrio
    };
}
