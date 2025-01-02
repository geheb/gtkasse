using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GtKasse.Cli.Models
{
    internal class ImportUserModel
    {
        [Index(0)]
        public string? DebtorNumber { get; set; }

        [Index(1)]
        public string? AddressNumber { get; set; }

        [Index(2)]
        public string? FirstName { get; set; }

        [Index(3)]
        public string? LastName { get; set; }

        [Index(4)]
        public string? EmailAddress { get; set; }
    }
}
