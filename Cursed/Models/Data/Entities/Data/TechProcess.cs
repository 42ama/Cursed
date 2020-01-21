using System;
using System.ComponentModel.DataAnnotations;

namespace Cursed.Models.Entities.Data
{
    public partial class TechProcess
    {
        [Required]
        public int FacilityId { get; set; }
        [Required]
        public int RecipeId { get; set; }
        [Range(1, Int32.MaxValue, ErrorMessage = "Day efficiency must be positive")]
        public decimal DayEfficiency { get; set; }

        public virtual Facility Facility { get; set; }
        public virtual Recipe Recipe { get; set; }
    }
}
