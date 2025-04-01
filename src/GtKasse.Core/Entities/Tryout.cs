namespace GtKasse.Core.Entities;

internal sealed class Tryout
{
    public Guid Id { get; set; }
    public string? Type { get; set; }
    public DateTimeOffset Date { get; set; }
    public Guid? UserId { get; set; }
    public IdentityUserGuid? User { get; set; }
    public int MaxBookings { get; set; }
    public DateTimeOffset? BookingStart { get; set; }
    public DateTimeOffset? BookingEnd { get; set; }
    public string? Description { get; set; }

    internal ICollection<TryoutBooking>? TryoutBookings { get; set; }
}
