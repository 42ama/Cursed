using System.Collections.Generic;
using Cursed.Models.Entities.Data;

namespace Cursed.Models.DataModel.Facilities
{
    public class FacilitiesModel : FacilitiesAbstractModel
    {
        public List<TechProcess> TechProcesses { get; set; }
    }
}
