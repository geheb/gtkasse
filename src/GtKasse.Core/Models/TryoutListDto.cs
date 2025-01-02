using GtKasse.Core.Converter;
using GtKasse.Core.Entities;

namespace GtKasse.Core.Models;

public sealed class TryoutListDto
{
    public Guid Id { get; set; }
    public DateTimeOffset Date { get; set; }
    public string? ContactName { get; set; }
    public string? ContactEmail { get; set; }
    public int BookingCount { get; }
    public int MaxBookings { get; }
    public bool IsExpired { get; }
    public string? Description { get; set; }
    public bool CanAccept { get; }

    internal TryoutListDto(Tryout tryout, int bookingCount, GermanDateTimeConverter dc)
    {
        Id = tryout.Id;
        Date = dc.ToLocal(tryout.Date);
        ContactName = tryout.User?.Name;
        ContactEmail = tryout.User?.EmailConfirmed == true ? tryout.User.Email : null;
        BookingCount = bookingCount;

        MaxBookings = tryout.MaxBookings;
        IsExpired = tryout.Date < DateTimeOffset.UtcNow;

        Description = tryout.Description;

        var canBook =
            DateTimeOffset.UtcNow >= tryout.BookingStart &&
            DateTimeOffset.UtcNow <= tryout.BookingEnd;

        CanAccept = canBook && bookingCount < tryout.MaxBookings;
    }
}
