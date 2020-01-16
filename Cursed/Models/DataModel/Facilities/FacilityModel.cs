using System.Collections.Generic;
using Cursed.Models.Entities;
using Cursed.Models.DataModel.Shared;

namespace Cursed.Models.DataModel.Facilities
{
    public class FacilityModel : FacilitiesAbstractModel
    {
        public List<FacilitiesProductContainer> Products { get; set; }
    }
}
