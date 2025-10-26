namespace GtKasse.Core.Entities;

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("vehicles")]
internal sealed class Vehicle
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(64)]
    public string? Name { get; set; }

    [Required]
    [MaxLength(12)]
    public string? Identifier { get; set; }

    public bool IsInUse { get; set; }

    public ICollection<VehicleBooking>? Bookings { get; set; }
}
