using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GtKasse.Core.Entities;

[Table("food_bookings")]
internal sealed class FoodBooking
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; }

    public Guid? UserId { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public IdentityUserGuid? User { get; set; }

    public Guid? FoodId { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public Food? Food { get; set; }

    [Required]
    public int Status { get; set; }

    [Required]
    public int Count { get; set; }

    [Required]
    public DateTimeOffset BookedOn { get; set; }

    public DateTimeOffset? CancelledOn { get; set; }

    public Guid? InvoiceId { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public Invoice? Invoice { get; set; }
}
