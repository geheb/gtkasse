using GtKasse.Core.Converter;
using GtKasse.Core.Entities;

namespace GtKasse.Core.Models;

public sealed class MyBoatRentalListDto
{
    public string? Purpose { get; set; }
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
    public bool IsFinished { get; set; }
    public bool IsCancelled { get; set; }

    public MyBoatRentalListDto(BoatRental entity, GermanDateTimeConverter dc)
    {
        Purpose = entity.Purpose;
        Start = dc.ToLocal(entity.Start);
        End = dc.ToLocal(entity.End);
        IsFinished = entity.End < DateTimeOffset.UtcNow;
        IsCancelled = entity.CancelledOn is not null;
    }
}
