using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cursed.Models.Data.Licenses
{
    public class LicensesViewModel : LicensesAbstractModel
    {
        public string Date { get; set; }
        public bool IsValid { get; set; }
        public IEnumerable<LicensesViewModel> RelatedLicenses { get; set; }
    }
}
