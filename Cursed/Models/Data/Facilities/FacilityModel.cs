using System.Collections.Generic;
using Cursed.Models.Entities;
using Cursed.Models.Data.Shared;

namespace Cursed.Models.Data.Facilities
{
    public class FacilityModel : FacilitiesAbstractModel
    {
        public List<FacilitiesProductContainer> Products { get; set; }
    }
}
