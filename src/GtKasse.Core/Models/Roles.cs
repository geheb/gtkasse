using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GtKasse.Core.Models
{
    public static class Roles
    {
        public const string Admin = "administrator";
        public const string Treasurer = "treasurer"; // Kassenwart
        public const string Kitchen = "kitchen"; // Küchendienst
        public const string Member = "member"; // Vereinsmitglied
        public const string Interested = "interested"; // Interessent
        public const string TripManager = "tripmanager"; // Fahrtenplaner
        public const string UserManager = "usermanager"; // Benutzermanager
        public const string Chairperson = "chairperson"; // Vorsitzende
        public const string FleetManager = "fleetmanager"; // Fahrzeugwart
        public const string BoatManager = "boatmanager"; // Bootswart
    }
}
