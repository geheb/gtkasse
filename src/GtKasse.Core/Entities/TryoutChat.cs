using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GtKasse.Core.Entities;

[Table("tryout_chats")]
internal sealed class TryoutChat
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; }

    public Guid? TryoutId { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public Tryout? Tryout { get; set; }


    public Guid? UserId { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public IdentityUserGuid? User { get; set; }

    [Required]
    public DateTimeOffset CreatedOn { get; set; }

    [Required]
    [MaxLength(256)]
    public string? Message { get; set; }
}
