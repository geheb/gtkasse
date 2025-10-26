using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GtKasse.Core.Entities;

[Table("foods")]
internal sealed class Food
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; }

    [MaxLength(256)]
    public string? Name { get; set; }

    [Precision(6, 2)]
    public decimal Price { get; set; }

    [Required]
    public int Type { get; set; }

    public Guid? FoodListId { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public FoodList? FoodList { get; set; }

    public ICollection<FoodBooking>? FoodBookings { get; set; }
}
