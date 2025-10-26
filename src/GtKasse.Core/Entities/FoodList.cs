using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GtKasse.Core.Entities;

[Table("food_lists")]
internal sealed class FoodList
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; }

    [MaxLength(256)]
    public string? Name { get; set; }

    [Required]
    public DateTimeOffset ValidFrom { get; set; }

    public ICollection<Food>? Foods { get; set; }
}
