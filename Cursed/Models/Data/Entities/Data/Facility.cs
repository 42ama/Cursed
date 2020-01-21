using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;

namespace Cursed.Models.Entities.Data
{
    public partial class Facility
    {
        public Facility()
        {
            TechProcess = new HashSet<TechProcess>();
        }
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Range(-90, 90, ErrorMessage = "Latitude must be in range between -90 and 90")]
        public decimal? Latitude { get; set; }
        [Range(-90, 90, ErrorMessage = "Longitude must be in range between -90 and 90")]
        public decimal? Longitude { get; set; }

        public virtual ICollection<TechProcess> TechProcess { get; set; }
    }
}
