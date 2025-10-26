using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GtKasse.Core.Entities;

[Table("invoices")]
internal sealed class Invoice
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; }

    public Guid? UserId { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public IdentityUserGuid? User { get; set; }

    [Required]
    public DateTimeOffset CreatedOn { get; set; }

    [Required]
    [Precision(6, 2)]
    public decimal Total { get; set; }

    [Required]
    public int Status { get; set; }

    public DateTimeOffset? PaidOn { get; set; }

    public Guid? InvoicePeriodId { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public InvoicePeriod? InvoicePeriod { get; set; }

    public ICollection<FoodBooking>? FoodBookings { get; set; }
}
