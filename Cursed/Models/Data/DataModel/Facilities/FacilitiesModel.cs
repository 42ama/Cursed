using System.Collections.Generic;
using Cursed.Models.Entities.Data;

namespace Cursed.Models.DataModel.Facilities
{
    /// <summary>
    /// Model used for multiple facilities data gathering
    /// </summary>
    public class FacilitiesModel : FacilitiesAbstractModel
    {
        public List<TechProcess> TechProcesses { get; set; }
    }
}
