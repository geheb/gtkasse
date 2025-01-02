using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GtKasse.Core.Entities
{
    internal sealed class AccountNotification
    {
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public IdentityUserGuid? User { get; set; }
        public int Type { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset? SentOn { get; set; }
        public string? CallbackUrl { get; set; }
        public Guid? ReferenceId { get; set; }
    }
}
