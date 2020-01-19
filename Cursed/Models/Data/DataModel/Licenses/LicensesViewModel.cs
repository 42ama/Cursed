using System.Collections.Generic;

namespace Cursed.Models.DataModel.Licenses
{
    /// <summary>
    /// Model used as base for licenses data presenting 
    /// </summary>
    public class LicensesViewModel : LicensesAbstractModel
    {
        public string Date { get; set; }
        public bool IsValid { get; set; }
        public IEnumerable<LicensesViewModel> RelatedLicenses { get; set; }
    }
}
