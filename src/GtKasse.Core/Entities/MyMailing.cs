using GtKasse.Core.Converter;
using GtKasse.Core.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GtKasse.Core.Entities;

[Table("my_mailings")]
public sealed class MyMailing : IEntity, IDtoMapper<MyMailingDto>
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; }

    [Required]
    public DateTimeOffset Created { get; set; }

    public DateTimeOffset? Updated { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public Mailing? Mailing { get; set; }

    public Guid? MailingId { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public IdentityUserGuid? User {  get; set; }

    public Guid? UserId { get; set; }

    public bool HasRead { get; set; }

    public void FromDto(MyMailingDto model)
    {
        Id = model.Id;
        MailingId = model.MailingId;
        UserId = model.UserId;
        HasRead = model.HasRead;
    }

    public MyMailingDto ToDto(GermanDateTimeConverter dc)
    {
        return new()
        {
            Id = Id,
            MailingId = MailingId,
            UserId = UserId,
            Created = dc.ToLocal(Created),
            ReplyAddress = Mailing!.ReplyAddress,
            Subject = Mailing!.Subject,
            HtmlBody = Mailing.HtmlBody,
            HasRead = HasRead,
        };
    }
}
