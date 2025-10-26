using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GtKasse.Core.Entities;

[Table("tryouts")]
internal sealed class Tryout
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(64)]
    public string? Type { get; set; }

    [Required]
    public DateTimeOffset Date { get; set; }

    public Guid? UserId { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public IdentityUserGuid? User { get; set; }

    [Required]
    public int MaxBookings { get; set; }

    [Required]
    public DateTimeOffset? BookingStart { get; set; }

    [Required]
    public DateTimeOffset? BookingEnd { get; set; }

    public string? Description { get; set; }

    [NotMapped]
    public bool IsExpired => DateTimeOffset.UtcNow > Date;

    public ICollection<TryoutBooking>? TryoutBookings { get; set; }
    public ICollection<TryoutChat>? TryoutChats { get; set; }
}
