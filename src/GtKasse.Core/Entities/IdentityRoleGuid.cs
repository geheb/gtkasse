using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GtKasse.Core.Entities
{
    public sealed class IdentityRoleGuid : IdentityRole<Guid>
    {
        public ICollection<IdentityUserRoleGuid>? UserRoles { get; set; }
    }
}
