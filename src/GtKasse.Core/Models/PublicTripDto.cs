using GtKasse.Core.Converter;
using GtKasse.Core.Entities;

namespace GtKasse.Core.Models;

public sealed class PublicTripDto
{
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
    public string? Target { get; set; }
    public TripCategory Categories { get; set; }

    internal PublicTripDto(Trip entity, GermanDateTimeConverter dc)
    {
        Start = dc.ToLocal(entity.Start);
        End = dc.ToLocal(entity.End);
        Target = entity.Target;
        Categories = (TripCategory)entity.Categories;
    }
}
