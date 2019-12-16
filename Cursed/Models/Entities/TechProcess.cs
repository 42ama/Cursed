using System;
using System.Collections.Generic;

namespace Cursed.Models.Entities
{
    public partial class TechProcess
    {
        public int FacilityId { get; set; }
        public int RecipeId { get; set; }
        public decimal DayEfficiency { get; set; }

        public virtual Facility Facility { get; set; }
        public virtual Recipe Recipe { get; set; }
    }
}
