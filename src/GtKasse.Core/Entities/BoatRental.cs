using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GtKasse.Core.Entities;

[Table("boat_rentals")]
public sealed class BoatRental
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; }

    public Guid? BoatId { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public Boat? Boat { get; set; }

    public Guid? UserId { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public IdentityUserGuid? User { get; set; }

    [Required]
    public DateTimeOffset Start { get; set; }

    [Required]
    public DateTimeOffset End { get; set; }

    [Required]
    [MaxLength(256)]
    public string? Purpose {  get; set; }

    public DateTimeOffset? CancelledOn { get; set; }
}
