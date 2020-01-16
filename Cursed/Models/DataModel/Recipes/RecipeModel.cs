using System;
using System.Collections.Generic;
using System.Linq;
using Cursed.Models.Entities.Data;
using Cursed.Models.DataModel.Shared;

namespace Cursed.Models.DataModel.Recipes
{
    public class RecipeModel : RecipesAbstractModel
    {
        public List<RecipeProductContainer> RecipeProducts { get; set; }
        public List<int> ChildRecipes { get; set; }
        public IEnumerable<Facility> RelatedFacilities { get; set; }
    }
}
