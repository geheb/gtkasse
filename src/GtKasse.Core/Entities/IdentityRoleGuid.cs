using Microsoft.AspNetCore.Identity;

namespace GtKasse.Core.Entities;

public sealed class IdentityRoleGuid : IdentityRole<Guid>
{
    public ICollection<IdentityUserRoleGuid>? UserRoles { get; set; }
}
