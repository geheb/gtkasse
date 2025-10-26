using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GtKasse.Core.Entities;

[Table("clubhouse_bookings")]
internal sealed class ClubhouseBooking
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; }

    [Required]
    public DateTimeOffset Start { get; set; }

    [Required]
    public DateTimeOffset End { get; set; }

    [Required]
    [MaxLength(256)]
    public string? Title { get; set; }

    public string? Description { get; set; }

    [NotMapped]
    public bool IsExpired => End < DateTimeOffset.UtcNow;
}
