using GtKasse.Core.Converter;
using GtKasse.Core.Entities;
using System.Globalization;

namespace GtKasse.Core.Models;

public class UserDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public bool EmailConfirmed { get; }
    public string? PhoneNumber { get; set; }
    public DateTimeOffset? LastLogin { get; }
    public string[]? Roles { get; set; }
    public string? DebtorNumber { get; set; }
    public string? AddressNumber { get; set; }
    public bool IsEmailConfirmed { get; set; }
    public bool IsLocked { get; set; }
    public bool CanBeDeleted { get; set; }

    public bool IsEnabledForBookings => 
        !(string.IsNullOrWhiteSpace(DebtorNumber) && string.IsNullOrWhiteSpace(AddressNumber));

    public UserDto()
    {
    }

    internal UserDto(IdentityUserGuid entity, bool canBeDeleted, IdnMapping idn, GermanDateTimeConverter dc)
    {
        idn ??= new IdnMapping();

        Id = entity.Id;
        Name = entity.Name;

        var email = entity.Email!.Split('@');
        Email = email[0] + "@" + idn.GetUnicode(email[1]);

        PhoneNumber = entity.PhoneNumber;

        EmailConfirmed = entity.EmailConfirmed;
        LastLogin = entity.LastLogin.HasValue ? dc.ToLocal(entity.LastLogin.Value) : null;
        Roles = entity.UserRoles?.Where(e => e.Role?.Name != null).Select(e => e.Role!.Name!).ToArray();

        DebtorNumber = entity.DebtorNumber;
        AddressNumber = entity.AddressNumber;

        IsEmailConfirmed = entity.EmailConfirmed;
        IsLocked = entity.LockoutEnabled && entity.LockoutEnd.HasValue && entity.LockoutEnd.Value > DateTimeOffset.UtcNow;

        CanBeDeleted = canBeDeleted;
    }
}
