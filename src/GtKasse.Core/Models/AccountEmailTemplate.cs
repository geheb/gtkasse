using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GtKasse.Core.Models
{
    public enum AccountEmailTemplate
    {
        ConfirmRegistration = 0,
        ConfirmPasswordForgotten = 1,
        ConfirmChangeEmail = 2,
        ConfirmRegistrationExtended = 3
    }
}
