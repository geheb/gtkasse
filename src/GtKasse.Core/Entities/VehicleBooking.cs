namespace GtKasse.Core.Entities;

using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("vehicle_bookings")]
internal sealed class VehicleBooking
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; }

    public Guid? VehicleId { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public Vehicle? Vehicle { get; set; }

    public Guid? UserId { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public IdentityUserGuid? User { get; set; }

    [Required]
    public DateTimeOffset Start { get; set; }

    [Required]
    public DateTimeOffset End { get; set; }

    [Required]
    public DateTimeOffset BookedOn { get; set; }

    public DateTimeOffset? ConfirmedOn { get; set; }

    public DateTimeOffset? CancelledOn { get; set; }

    [Required]
    [MaxLength(256)]
    public string? Purpose { get; set; }

    [NotMapped]
    public bool IsExpired => End < DateTimeOffset.UtcNow;
}
