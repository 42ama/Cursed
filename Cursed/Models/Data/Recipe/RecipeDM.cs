using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cursed.Models.Data.Shared;

namespace Cursed.Models.Data.Recipe
{
    public class RecipeDM
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public bool? TechApproved { get; set; }
        public bool? GovApproved { get; set; }
        public List<RecipeProductContainer> RecipeProducts { get; set; }
        public int? ParentRecipe { get; set; }
        public List<int> ChildRecipes { get; set; }

    }
}
