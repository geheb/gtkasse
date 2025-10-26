using Microsoft.AspNetCore.Identity;

namespace GtKasse.Core.Entities;

public sealed class IdentityUserRoleGuid : IdentityUserRole<Guid>
{
    public IdentityUserGuid? User { get; set; }

    public IdentityRoleGuid? Role { get; set; }
}
