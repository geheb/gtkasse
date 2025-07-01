using GtKasse.Core.Converter;
using GtKasse.Core.Models;
using Microsoft.AspNetCore.Identity;
using System.Globalization;

namespace GtKasse.Core.Entities
{
    public sealed class IdentityUserGuid : IdentityUser<Guid>
    {
        public string? Name { get; set; }
        public DateTimeOffset? LastLogin { get; set; }
        public DateTimeOffset? LeftOn { get; set; }
        public string? DebtorNumber { get; set; }
        public string? AddressNumber { get; set; }
        public string? AuthenticatorKey { get; set; }

        internal ICollection<IdentityUserRoleGuid>? UserRoles { get; set; }
        internal ICollection<Booking>? Bookings { get; set; }
        internal ICollection<Invoice>? Invoices { get; set; }
        internal ICollection<Trip>? Trips { get; set; }
        internal ICollection<TripBooking>? TripBookings { get; set; }
        internal ICollection<WikiArticle>? WikiArticles { get; set; }
        internal ICollection<TripChat>? TripChats { get; set; }
        internal ICollection<VehicleBooking>? VehicleBookings { get; set; }
        internal ICollection<Tryout>? Tryouts { get; set; }
        internal ICollection<TryoutBooking>? TryoutBookings {  get; set; }
        internal ICollection<BoatRental>? BoatRentals { get; set; }
        internal ICollection<TryoutChat>? TryoutChats { get; set; }

        public IdentityDto ToDto(GermanDateTimeConverter dc)
        {
            return new()
            {
                Id = Id,
                Name = Name,
                Email = Email,
                PhoneNumber = PhoneNumber,
                IsEmailConfirmed = EmailConfirmed,
                LastLogin = LastLogin.HasValue ? dc.ToLocal(LastLogin.Value) : null,
                Roles = UserRoles?.Where(e => e.Role?.Name != null).Select(e => e.Role!.Name!).ToArray(),
                DebtorNumber = DebtorNumber,
                AddressNumber = AddressNumber,
                IsLocked = LockoutEnabled && LockoutEnd.HasValue && LockoutEnd.Value > DateTimeOffset.UtcNow
            };
        }
    }
}
