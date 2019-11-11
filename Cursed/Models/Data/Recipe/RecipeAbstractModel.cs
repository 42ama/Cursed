using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cursed.Models.Data.Recipe
{
    public abstract class RecipeAbstractModel
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public bool? TechApproved { get; set; }
        public bool? GovApproved { get; set; }
        public int? ParentRecipe { get; set; }
    }
}
