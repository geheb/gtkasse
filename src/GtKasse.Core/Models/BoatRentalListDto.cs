namespace GtKasse.Core.Models;

public sealed class BoatRentalListDto
{
    public Guid Id { get; set; }
    public BoatDto? Boat { get; set; }
    public int Count { get; set; }
}
