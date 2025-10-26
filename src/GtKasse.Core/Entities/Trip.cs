using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GtKasse.Core.Entities;

[Table("trips")]
internal class Trip
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; }

    [Required]
    public DateTimeOffset Start { get; set; }

    [Required]
    public DateTimeOffset End { get; set; }

    public Guid? UserId { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public IdentityUserGuid? User { get; set; }

    [Required]
    public string? Target { get; set; }

    [Required]
    public int MaxBookings { get; set; }

    [Required]
    public DateTimeOffset? BookingStart { get; set; }

    [Required]
    public DateTimeOffset? BookingEnd { get; set; }

    public string? Description { get; set; }

    [NotMapped]
    public bool IsExpired => DateTimeOffset.UtcNow > End;

    public int Categories { get; set; }

    public bool IsPublic { get; set; }

    public ICollection<TripBooking>? TripBookings { get; set; }
    public ICollection<TripChat>? TripChats { get; set; }
}
