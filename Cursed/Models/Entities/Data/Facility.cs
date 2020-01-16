using System;
using System.Collections.Generic;

namespace Cursed.Models.Entities.Data
{
    public partial class Facility
    {
        public Facility()
        {
            TechProcess = new HashSet<TechProcess>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }

        public virtual ICollection<TechProcess> TechProcess { get; set; }
    }
}
