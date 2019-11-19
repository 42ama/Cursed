using System.Collections.Generic;
using Cursed.Models.Entities;

namespace Cursed.Models.Data.Facilities
{
    public class FacilitiesModel : FacilitiesAbstractModel
    {
        public List<TechProcess> TechProcesses { get; set; }
    }
}
