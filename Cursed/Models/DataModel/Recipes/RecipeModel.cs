using System.Collections.Generic;
using Cursed.Models.Entities.Data;
using Cursed.Models.DataModel.RecipeProducts;

namespace Cursed.Models.DataModel.Recipes
{
    public class RecipeModel : RecipesAbstractModel
    {
        public List<RecipeProductContainer> RecipeProducts { get; set; }
        public List<int> ChildRecipes { get; set; }
        public IEnumerable<Facility> RelatedFacilities { get; set; }
    }
}
