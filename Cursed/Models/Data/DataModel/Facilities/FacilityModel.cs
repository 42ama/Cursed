using System.Collections.Generic;

namespace Cursed.Models.DataModel.Facilities
{
    /// <summary>
    /// Model used for single facility data gathering
    /// </summary>
    public class FacilityModel : FacilitiesAbstractModel
    {
        public List<FacilitiesProductContainer> Products { get; set; }
    }
}
