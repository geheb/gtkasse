using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GtKasse.Core.Entities;

[Table("trip_bookings")]
internal class TripBooking
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

    [MaxLength(256)]
    public string? Name { get; set; }

    [Required]
    public DateTimeOffset BookedOn { get; set; }

    public DateTimeOffset? ConfirmedOn { get; set; }

    public DateTimeOffset? CancelledOn { get; set; }
}
