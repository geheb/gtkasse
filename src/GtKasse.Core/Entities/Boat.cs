using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GtKasse.Core.Entities;

[Table("boats")]
public sealed class Boat
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(64)]
    public string? Name { get; set; }

    [Required]
    [MaxLength(8)]
    public string? Identifier { get; set; }

    [Required]
    [MaxLength(64)]
    public string? Location { get; set; }

    public int MaxRentalDays { get; set; }

    public bool IsLocked { get; set; }

    public string? Description { get; set; }

    public ICollection<BoatRental>? BoatRentals { get; set; }
}
