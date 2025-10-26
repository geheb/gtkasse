namespace GtKasse.Core.Entities; 

using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("trip_chats")]
internal sealed class TripChat
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; }

    public Guid? TripId { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public Trip? Trip { get; set; }

    public Guid? UserId { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public IdentityUserGuid? User { get; set; }

    [Required]
    public DateTimeOffset CreatedOn { get; set; }

    [Required]
    [MaxLength(256)]
    public string? Message { get; set; }
}
